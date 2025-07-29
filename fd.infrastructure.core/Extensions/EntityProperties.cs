using fd.infrastructure.core.Utilities;
using fd.infrastructure.entity;
using fd.infrastructure.entity.SysModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static class EntityProperties
    {
        private static readonly Dictionary<Type, string> ProperWithDbType = new Dictionary<Type, string>() {
            {  typeof(string),SqlDbTypeName.NVarChar },
            { typeof(DateTime),SqlDbTypeName.DateTime},
            {typeof(long),SqlDbTypeName.BigInt },
            {typeof(int),SqlDbTypeName.Int},
            { typeof(decimal),SqlDbTypeName.Decimal },
            { typeof(float),SqlDbTypeName.Float },
            { typeof(double),SqlDbTypeName.Double },
            {  typeof(byte),SqlDbTypeName.Int },//类型待完
            { typeof(Guid),SqlDbTypeName.UniqueIdentifier}
        };

        /// <summary>
        /// 验证数据库字段类型与值是否正确，
        /// </summary>
        /// <param name="propertyInfo">propertyInfo为当字段，当前字段必须有ColumnAttribute属性,
        /// 如字段:标识为数据库int类型[Column(TypeName="int")]  public int Id { get; set; }
        /// 如果是小数float或Decimal必须对propertyInfo字段加DisplayFormatAttribute属性
        /// </param>
        /// <param name="value"></param>
        /// <returns>IEnumerable<(bool, string, object)> bool成否校验成功,string校验失败信息,object,当前校验的值</returns>
        public static IEnumerable<(bool, string, object)> ValidationValueForDbType(this PropertyInfo propertyInfo, params object[] values)
        {
            string dbTypeName = propertyInfo.GetTypeCustomValue<ColumnAttribute>(c => c.TypeName);
            foreach (object value in values)
            {
                yield return dbTypeName.ValidationVal(value, propertyInfo);
            }
        }

        private static string[] _userEditFields { get; set; }

        private static string[] UserEditFields
        {
            get
            {
                if (_userEditFields != null) return _userEditFields;
                _userEditFields = AppSetting.CreateMember.GetType().GetProperties()
                     .Select(x => x.GetValue(AppSetting.ModifyMember)?.ToString()?.ToLower())
                     .Where(w => !string.IsNullOrEmpty(w)).ToArray();
                return _userEditFields;
            }
        }


        /// <summary>
        /// 获取类的单个指定属性的值(只会返回第一个属性的值)
        /// </summary>
        /// <param name="member">当前类</param>
        /// <param name="type">指定的类</param>
        /// <param name="expression">指定属性的值 格式 Expression<Func<entityt, object>> exp = x => new { x.字段1, x.字段2 };</param>
        /// <returns></returns>
        public static string GetTypeCustomValue<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
        {
            var propertyKeyValues = member.GetTypeCustomValues(expression);
            if (propertyKeyValues == null || propertyKeyValues.Count == 0)
            {
                return null;
            }
            return propertyKeyValues.First().Value ?? "";
        }

        /// <summary>
        /// 获取类的多个指定属性的值
        /// </summary>
        /// <param name="member">当前类</param>
        /// <param name="type">指定的类</param>
        /// <param name="expression">指定属性的值 格式 Expression<Func<entityt, object>> exp = x => new { x.字段1, x.字段2 };</param>
        /// <returns>返回的是字段+value</returns>
        public static Dictionary<string, string> GetTypeCustomValues<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
        {
            var attr = member.GetTypeCustomAttributes(typeof(TEntity));
            if (attr == null)
            {
                return null;
            }

            string[] propertyName = expression.GetExpressionProperty();
            Dictionary<string, string> propertyKeyValues = new Dictionary<string, string>();

            foreach (PropertyInfo property in attr.GetType().GetProperties())
            {
                if (propertyName.Contains(property.Name))
                {
                    propertyKeyValues[property.Name] = (property.GetValue(attr) ?? string.Empty).ToString();
                }
            }
            return propertyKeyValues;
        }


        /// <summary>
        /// 设置默认字段的值"CreateID", "Creator", "CreateDate"，"ModifyID", "Modifier", "ModifyDate"
        /// </summary>
        /// <param name="saveDataModel"></param>
        /// <param name="setType">true=新增设置"CreateID", "Creator", "CreateDate"值
        /// false=编辑设置"ModifyID", "Modifier", "ModifyDate"值
        /// </param>
        public static SaveModel SetDefaultVal(this SaveModel saveDataModel, TableDefaultColumns defaultColumns, UserContext userInfo = null)
        {
            SetDefaultVal(saveDataModel.main_data, defaultColumns, userInfo);
            if (saveDataModel.DetailData != null && saveDataModel.DetailData.Count > 0)
            {
                foreach (var item in saveDataModel.DetailData)
                {
                    if (item.Count == 0) continue;
                    SetDefaultVal(item, defaultColumns, userInfo);
                }
            }
            return saveDataModel;
        }

        public static TSource SetCreateDefaultVal<TSource>(this TSource source, UserContext user = null)
        {
            return SetDefaultVal(source, AppSetting.CreateMember, user);
        }

        private static TSource SetDefaultVal<TSource>(this TSource source, TableDefaultColumns defaultColumns, UserContext user = null)
        {            
            foreach (PropertyInfo property in typeof(TSource).GetProperties())
            {
                string filed = property.Name.ToLower();
                if (filed == defaultColumns.UserIdField?.ToLower())
                    property.SetValue(source, user.user_id.ChangeType(property.PropertyType));

                if (filed == defaultColumns.UserNameField?.ToLower())
                    property.SetValue(source, user.user_name);

                if (filed == defaultColumns.DateField?.ToLower())
                    property.SetValue(source, DateTime.Now);
            }
            return source;
        }


        private static Dictionary<string, object> SetDefaultVal(this Dictionary<string, object> dic, TableDefaultColumns defaultColumns, UserContext userInfo = null)
        {
            
            KeyValuePair<string, object> valuePair = dic.Where(x => x.Key.ToLower() == defaultColumns.UserIdField?.ToLower()).FirstOrDefault();

            if (valuePair.Key != null || defaultColumns.UserIdField != null)
            {
                dic[valuePair.Key ?? defaultColumns.UserIdField] = userInfo.user_id;
            }

            valuePair = dic.Where(x => x.Key.ToLower() == defaultColumns.UserNameField?.ToLower()).FirstOrDefault();
            if (valuePair.Key != null || defaultColumns.UserNameField != null)
            {
                dic[valuePair.Key ?? defaultColumns.UserNameField] = userInfo.user_name;
            }

            valuePair = dic.Where(x => x.Key.ToLower() == defaultColumns.DateField?.ToLower()).FirstOrDefault();
            if (valuePair.Key != null || defaultColumns.DateField != null)
            {
                dic[valuePair.Key ?? defaultColumns.DateField] = DateTime.Now;
            }

            return dic;
        }



        /// <summary>
        /// 获取主键字段
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static PropertyInfo GetKeyProperty(this Type entity)
        {
            return entity.GetProperties().GetKeyProperty();
        }

        public static PropertyInfo GetKeyProperty(this PropertyInfo[] properties)
        {
            return properties.Where(c => c.IsKey()).FirstOrDefault();
        }

        public static bool IsKey(this PropertyInfo propertyInfo)
        {
            object[] keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false);
            if (keyAttributes.Length > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 获取属性的指定属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTypeCustomAttributes(this MemberInfo member, Type type)
        {
            object[] obj = member.GetCustomAttributes(type, false);
            if (obj.Length == 0) return null;
            return obj[0];
        }


        /// <summary>
        /// 获取类的指定属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTypeCustomAttributes(this Type entity, Type type)
        {
            object[] obj = entity.GetCustomAttributes(type, false);
            if (obj.Length == 0) return null;
            return obj[0];
        }

        /// <summary>
        /// 获取PropertyInfo指定属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTypeCustomAttributes(this PropertyInfo propertyInfo, Type type, out bool asType)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(type, false);
            if (attributes.Length == 0)
            {
                asType = false;
                return new string[0];
            }
            asType = true;
            return attributes[0];
        }

        /// <summary>
        /// 获取对象里指定成员名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="properties"> 格式 Expression<Func<entityt, object>> exp = x => new { x.字段1, x.字段2 };或x=>x.Name</param>
        /// <returns></returns>
        public static string[] GetExpressionProperty<TEntity>(this Expression<Func<TEntity, object>> properties)
        {
            if (properties == null)
                return new string[] { };
            if (properties.Body is NewExpression)
                return ((NewExpression)properties.Body).Members.Select(x => x.Name).ToArray();
            if (properties.Body is MemberExpression)
                return new string[] { ((MemberExpression)properties.Body).Member.Name };
            if (properties.Body is UnaryExpression)
                return new string[] { ((properties.Body as UnaryExpression).Operand as MemberExpression).Member.Name };
            throw new Exception("未实现的表达式");
        }

        /// <summary>
        /// 验证数据库字段类型与值是否正确，
        /// </summary>
        /// <param name="dbType">数据库字段类型(如varchar,nvarchar,decimal,不要带后面长度如:varchar(50))</param>
        /// <param name="value">值</param>
        /// <param name="propertyInfo">要验证的类的属性，若不为null，则会判断字符串的长度是否正确</param>
        /// <returns>(bool, string, object)bool成否校验成功,string校验失败信息,object,当前校验的值</returns>
        public static (bool, string, object) ValidationVal(this string dbType, object value, PropertyInfo propertyInfo = null)
        {
            if (string.IsNullOrEmpty(dbType))
            {
                dbType = propertyInfo != null ? propertyInfo.GetProperWithDbType() : SqlDbTypeName.NVarChar;
            }
            dbType = dbType.ToLower();
            string val = value?.ToString();
            //验证长度
            string reslutMsg = string.Empty;
            if (dbType == SqlDbTypeName.Int)
            {
                if (!value.IsInt())
                    reslutMsg = "只能为有效整数";
            }  //2021.10.12增加属性校验long类型的支持
            else if (dbType == SqlDbTypeName.BigInt)
            {
                if (!long.TryParse(val, out _))
                {
                    reslutMsg = "只能为有效整数";
                }
            }
            else if (dbType == SqlDbTypeName.DateTime
                || dbType == SqlDbTypeName.Date
                || dbType == SqlDbTypeName.SmallDateTime
                || dbType == SqlDbTypeName.SmallDate
                )
            {
                if (!value.IsDate())
                    reslutMsg = "必须为日期格式";
            }
            else if (dbType == SqlDbTypeName.Float || dbType == SqlDbTypeName.Decimal || dbType == SqlDbTypeName.Double)
            {
                //string formatString = string.Empty;
                //if (propertyInfo != null)
                //    formatString = propertyInfo.GetTypeCustomValue<DisplayFormatAttribute>(x => x.DataFormatString);
                //if (string.IsNullOrEmpty(formatString))
                //    throw new Exception("请对字段" + propertyInfo?.Name + "添加DisplayFormat属性标识");

                if (!val.IsNumber(null))
                {
                    // string[] arr = (formatString ?? "10,0").Split(',');
                    // reslutMsg = $"整数{arr[0]}最多位,小数最多{arr[1]}位";
                    reslutMsg = "不是有效数字";
                }
            }
            else if (dbType == SqlDbTypeName.UniqueIdentifier)
            {
                if (!val.IsGuid())
                {
                    reslutMsg = propertyInfo.Name + "Guid不正确";
                }
            }
            else if (propertyInfo != null
                && (dbType == SqlDbTypeName.VarChar
                || dbType == SqlDbTypeName.NVarChar
                || dbType == SqlDbTypeName.NChar
                || dbType == SqlDbTypeName.Char
                || dbType == SqlDbTypeName.Text))
            {

                //默认nvarchar(max) 、text 长度不能超过20000
                if (val.Length > 200000)
                {
                    reslutMsg = $"字符长度最多【200000】";
                }
                else
                {
                    int length = propertyInfo.GetTypeCustomValue<MaxLengthAttribute>(x => new { x.Length }).GetInt();
                    if (length == 0) { return (true, null, null); }
                    //判断双字节与单字段
                    else if (length < 8000 &&
                        ((dbType.Substring(0, 1) != "n"
                        && Encoding.UTF8.GetBytes(val.ToCharArray()).Length > length)
                         || val.Length > length)
                         )
                    {
                        reslutMsg = $"最多只能【{length}】个字符。";
                    }
                }
            }
            if (!string.IsNullOrEmpty(reslutMsg) && propertyInfo != null)
            {
                reslutMsg = propertyInfo.GetDisplayName() + reslutMsg;
            }
            return (reslutMsg == "" ? true : false, reslutMsg, value);
        }

        public static string GetProperWithDbType(this PropertyInfo propertyInfo)
        {
            bool result = ProperWithDbType.TryGetValue(propertyInfo.PropertyType, out string value);
            if (result)
            {
                return value;
            }
            return SqlDbTypeName.NVarChar;
        }

        public static FieldType GetFieldType(this Type typeEntity)
        {
            FieldType fieldType;
            string columnType = typeEntity.GetProperties().Where(x => x.Name == typeEntity.GetKeyName()).ToList()[0].GetColumnType(false).Value;
            switch (columnType)
            {
                case SqlDbTypeName.Int: fieldType = FieldType.Int; break;
                case SqlDbTypeName.BigInt: fieldType = FieldType.BigInt; break;
                case SqlDbTypeName.VarChar: fieldType = FieldType.VarChar; break;
                case SqlDbTypeName.UniqueIdentifier: fieldType = FieldType.UniqueIdentifier; break;
                default: fieldType = FieldType.NvarChar; break;
            }
            return fieldType;
        }


        /// <summary>
        /// 获取表带有EntityAttribute属性的真实表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetEntityTableName(this Type type)
        {
            Attribute attribute = type.GetCustomAttribute(typeof(EntityAttribute));
            if (attribute != null && attribute is EntityAttribute)
            {
                return (attribute as EntityAttribute).TableName ?? type.Name;
            }
            return type.Name;
        }


        /// <summary>
        /// 指定需要验证的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="expression">对指定属性进行验证x=>{x.Name,x.Size}</param>
        /// <returns></returns>
        public static WebResponseContent ValidationEntity<T>(this T entity, Expression<Func<T, object>> expression = null, Expression<Func<T, object>> validateProperties = null)
        {
            return ValidationEntity<T>(entity, expression?.GetExpressionProperty<T>(), validateProperties?.GetExpressionProperty<T>());
        }


        /// <summary>
        /// specificProperties=null并且validateProperties=null，对所有属性验证，只验证其是否合法，不验证是否为空(除属性标识指定了不能为空外)
        /// specificProperties!=null，对指定属性校验，并且都必须有值
        /// null并且validateProperties!=null，对指定属性校验，不判断是否有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="specificProperties">验证指定的属性，并且非空判断</param>
        /// <param name="validateProperties">验证指定属性，只对字段合法性判断，不验证是否为空</param>
        /// <returns></returns>
        public static WebResponseContent ValidationEntity<T>(this T entity, string[] specificProperties, string[] validateProperties = null)
        {
            WebResponseContent responseData = new WebResponseContent();
            if (entity == null) return responseData.Error("对象不能为null");

            PropertyInfo[] propertyArray = typeof(T).GetProperties();
            //若T为object取不到属性
            if (propertyArray.Length == 0)
            {
                propertyArray = entity.GetType().GetProperties();
            }
            List<PropertyInfo> compareProper = new List<PropertyInfo>();

            //只验证数据合法性，验证非空
            if (specificProperties != null && specificProperties.Length > 0)
            {
                compareProper.AddRange(propertyArray.Where(x => specificProperties.Contains(x.Name)));
            }

            //只验证数据合法性，不验证非空
            if (validateProperties != null && validateProperties.Length > 0)
            {
                compareProper.AddRange(propertyArray.Where(x => validateProperties.Contains(x.Name)));
            }
            if (compareProper.Count() > 0)
            {
                propertyArray = compareProper.ToArray();
            }
            foreach (PropertyInfo propertyInfo in propertyArray)
            {
                object value = propertyInfo.GetValue(entity);
                //设置默认状态的值
                if (propertyInfo.Name == "VALID")
                {
                    if (value == null)
                    {
                        propertyInfo.SetValue(entity, 0);
                        continue;
                    }
                }
                //若存在specificProperties并且属性为数组specificProperties中的值，校验时就需要判断是否为空
                var reslut = propertyInfo.ValidationProperty(value,
                    specificProperties != null && specificProperties.Contains(propertyInfo.Name) ? true : false
                    );
                if (!reslut.Item1)
                    return responseData.Error(reslut.Item2);
            }
            return responseData.OK("验证成功");
        }


        //public static TSource SetCreateDefaultVal<TSource>(this TSource source, UserInfo userInfo = null)
        //{
        //    return SetDefaultVal(source, AppSetting.CreateMember, userInfo);
        //}

        ///// <summary>
        ///// 
        ///// 设置默认字段的值如:"CreateID", "Creator", "CreateDate"，"ModifyID", "Modifier", "ModifyDate"
        ///// </summary>
        ///// <param name="saveDataModel"></param>
        ///// <param name="setType">true=新增设置"CreateID", "Creator", "CreateDate"值
        ///// false=编辑设置"ModifyID", "Modifier", "ModifyDate"值
        ///// </param>
        //private static TSource SetDefaultVal<TSource>(this TSource source, TableDefaultColumns defaultColumns, UserInfo userInfo = null)
        //{
        //    userInfo = userInfo ?? ManageUser.UserContext.Current.UserInfo;
        //    foreach (PropertyInfo property in typeof(TSource).GetProperties())
        //    {
        //        string filed = property.Name.ToLower();
        //        if (filed == defaultColumns.UserIdField?.ToLower())
        //            property.SetValue(source, userInfo.User_Id);

        //        if (filed == defaultColumns.UserNameField?.ToLower())
        //            property.SetValue(source, userInfo.UserTrueName);

        //        if (filed == defaultColumns.DateField?.ToLower())
        //            property.SetValue(source, DateTime.Now);
        //    }
        //    return source;
        //}


        public static string GetDisplayName(this PropertyInfo property)
        {
            string displayName = property.GetTypeCustomValue<DisplayAttribute>(x => new { x.Name });
            if (string.IsNullOrEmpty(displayName))
            {
                return property.Name;
            }
            return displayName;
        }

        public static string GetKeyName(this PropertyInfo[] properties)
        {
            return properties.GetKeyName(false);
        }
        /// <summary>
        /// 获取key列名
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="keyType">true获取key对应类型,false返回对象Key的名称</param>
        /// <returns></returns>
        public static string GetKeyName(this PropertyInfo[] properties, bool keyType)
        {
            string keyName = string.Empty;
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!propertyInfo.IsKey())
                    continue;
                if (!keyType)
                    return propertyInfo.Name;
                var attributes = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false);
                //如果没有ColumnAttribute的需要单独再验证，下面只验证有属性的
                if (attributes.Length > 0)
                    return ((ColumnAttribute)attributes[0]).TypeName.ToLower();
                else
                    return GetColumType(new PropertyInfo[] { propertyInfo }, true)[propertyInfo.Name];
            }
            return keyName;
        }

        /// <summary>
        /// 判断hash的列是否为对应的实体，并且值是否有效
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <param name="dic"></param>
        /// <param name="removeNotContains">移除不存在字段</param>
        /// <returns></returns>
        public static string ValidateDicInEntity(this Type typeinfo, Dictionary<string, object> dic, bool removeNotContains, string[] ignoreFields = null)
        {
            return typeinfo.ValidateDicInEntity(dic, removeNotContains, true, ignoreFields);
        }

        public static string ValidateDicInEntity(this Type type, Dictionary<string, object> dic, bool removeNotContains, bool removerKey, string[] ignoreFields = null)
        {
            return type.ValidateDicInEntity(dic, null, removeNotContains, removerKey, ignoreFields);
        }

        /// <summary>
        /// 判断hash的列是否为对应的实体，并且值是否有效
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <param name="dic"></param>
        /// <param name="removeNotContains">移除不存在字段</param>
        /// <param name="removerKey">移除主键</param>
        /// <returns></returns>
        private static string ValidateDicInEntity(this Type typeinfo, Dictionary<string, object> dic, PropertyInfo[] propertyInfo, bool removeNotContains, bool removerKey, string[] ignoreFields = null)
        {
            if (dic == null || dic.Count == 0) { return "参数无效"; }
            if (propertyInfo == null)
                propertyInfo = typeinfo.GetProperties().Where(x => x.PropertyType.Name != "List`1").ToArray();
            if (removeNotContains)
            {
                // 不存在的字段直接移除
                dic.Where(x => !propertyInfo.Any(p => p.Name == x.Key)).Select(s => s.Key).ToList().ForEach(f =>
                {
                    dic.Remove(f);
                });
            }
            string keyName = typeinfo.GetKeyName();
            //移除主键
            if (removerKey)
            {
                dic.Remove(keyName);
            }
            foreach (PropertyInfo property in propertyInfo)
            {
                //忽略与主键的字段不做验证
                if (property.Name == keyName || (ignoreFields != null && ignoreFields.Contains(property.Name)))
                    continue;

                //不在编辑中的列，是否也要必填
                if (!dic.ContainsKey(property.Name))
                {
                    //移除主键默认为新增数据，将不在编辑列中的有默认值的数据设置为默认值
                    //如果为true默认为添加功能，添加操作所有不能为空的列也必须要提交
                    if (property.GetCustomAttributes(typeof(RequiredAttribute)).Count() > 0
                        && property.PropertyType != typeof(int)
                        && property.PropertyType != typeof(long)
                        && property.PropertyType != typeof(byte)
                        && property.PropertyType != typeof(decimal)
                        )
                    {
                        return property.GetTypeCustomValue<DisplayAttribute>(x => x.Name) + "为必须提交项";
                    }
                    continue;
                }
                bool isEdit = property.ContainsCustomAttributes(typeof(EditableAttribute));
                //不是编辑列的直接移除,并且不是主键
                //removerKey=true，不保留主键，直接移除
                //removerKey=false,保留主键，属性与主键不同的直接移除
                //  if (!isEdit && (removerKey || (!removerKey && property.Name != keyName)))
                if (!isEdit)
                {
                    if (property.GetCustomAttributes(typeof(RequiredAttribute)).Count() > 0)
                    {
                        return property.GetTypeCustomValue<DisplayAttribute>(x => x.Name) + "没有配置好Model为编辑列";
                    }
                    dic.Remove(property.Name);
                    continue;
                }
                ////移除忽略的不保存的数据
                //if (property.ContainsCustomAttributes(typeof(JsonIgnoreAttribute)))
                //{
                //    hash.Remove(property.Name);
                //    continue;
                //}
                //验证数据类型,不验证是否为空
                var result = property.ValidationProperty(dic[property.Name], false);
                if (!result.Item1)
                    return result.Item2;

                //将所有空值设置为null
                if (dic[property.Name] != null && dic[property.Name].ToString() == string.Empty)
                    dic[property.Name] = null;
            }
            return string.Empty;
        }



        public static string ValidateDicInEntity(this Type type, List<Dictionary<string, object>> dicList, bool removeNotContains, bool removerKey, string[] ignoreFields = null)
        {
            PropertyInfo[] propertyInfo = type.GetProperties();
            string reslutMsg = string.Empty;
            foreach (Dictionary<string, object> dic in dicList)
            {
                reslutMsg = type.ValidateDicInEntity(dic, propertyInfo, removeNotContains, removerKey, ignoreFields);
                if (!string.IsNullOrEmpty(reslutMsg))
                    return reslutMsg;
            }
            return reslutMsg;
        }

        /// <summary>
        /// 判断是否包含某个属性：
        /// 如 [Editable(true)]
        //  public string MO { get; set; }包含Editable
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ContainsCustomAttributes(this PropertyInfo propertyInfo, Type type)
        {
            propertyInfo.GetTypeCustomAttributes(type, out bool contains);
            return contains;
        }

        public static Dictionary<string, string> GetColumType(this PropertyInfo[] properties, bool containsKey)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (PropertyInfo property in properties)
            {
                if (!containsKey && property.IsKey())
                {
                    continue;
                }
                var keyVal = GetColumnType(property, true);
                dictionary.Add(keyVal.Key, keyVal.Value);
            }
            return dictionary;
        }

        /// <summary>
        /// 获取实体所有可以编辑的列
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetEditField(this Type type)
        {
            Type editType = typeof(EditableAttribute);
            PropertyInfo[] propertyInfo = type.GetProperties();
            string keyName = propertyInfo.GetKeyName();
            return propertyInfo.Where(x => x.Name != keyName && (x.ContainsCustomAttributes(editType))).Select(s => s.Name).ToArray();
        }

        /// <summary>
        /// 验证每个属性的值是否正确
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="objectVal">属性的值</param>
        /// <param name="required">是否指定当前属性必须有值</param>
        /// <returns></returns>
        public static (bool, string, object) ValidationProperty(this PropertyInfo propertyInfo, object objectVal, bool required)
        {
            if (propertyInfo.IsKey()) { return (true, null, objectVal); }

            string val = objectVal == null ? "" : objectVal.ToString().Trim();

            string requiredMsg = string.Empty;
            if (!required)
            {
                var reuireVal = propertyInfo.GetTypeCustomValues<RequiredAttribute>(x => new { x.AllowEmptyStrings, x.ErrorMessage });
                if (reuireVal != null && !Convert.ToBoolean(reuireVal["AllowEmptyStrings"]))
                {
                    required = true;
                    requiredMsg = reuireVal["ErrorMessage"];
                }
            }
            //如果不要求为必填项并且值为空，直接返回
            if (!required && string.IsNullOrEmpty(val))
                return (true, null, objectVal);

            if ((required && val == string.Empty))
            {
                if (requiredMsg != "") return (false, requiredMsg, objectVal);
                string propertyName = propertyInfo.GetTypeCustomValue<DisplayAttribute>(x => new { x.Name });
                return (false, requiredMsg + (string.IsNullOrEmpty(propertyName) ? propertyInfo.Name : propertyName) + "不能为空", objectVal);
            }
            //列名
            string typeName = propertyInfo.GetSqlDbType();

            //如果没有ColumnAttribute的需要单独再验证，下面只验证有属性的
            if (typeName == null) { return (true, null, objectVal); }
            //验证长度
            return typeName.ValidationVal(val, propertyInfo);
        }


        /// <summary>
        /// 返回属性的字段及数据库类型
        /// </summary>
        /// <param name="property"></param>
        /// <param name="lenght">是否包括后字段具体长度:nvarchar(100)</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetColumnType(this PropertyInfo property, bool lenght = false)
        {
            string colType = "";
            object objAtrr = property.GetTypeCustomAttributes(typeof(ColumnAttribute), out bool asType);
            if (asType)
            {
                colType = ((ColumnAttribute)objAtrr).TypeName.ToLower();
                if (!string.IsNullOrEmpty(colType))
                {
                    //不需要具体长度直接返回
                    if (!lenght)
                    {
                        return new KeyValuePair<string, string>(property.Name, colType);
                    }
                    if (colType == "decimal" || colType == "double" || colType == "float")
                    {
                        objAtrr = property.GetTypeCustomAttributes(typeof(DisplayFormatAttribute), out asType);
                        colType += "(" + (asType ? ((DisplayFormatAttribute)objAtrr).DataFormatString : "18,5") + ")";

                    }
                    ///如果是string,根据 varchar或nvarchar判断最大长度
                    if (property.PropertyType.ToString() == "System.String")
                    {
                        colType = colType.Split("(")[0];
                        objAtrr = property.GetTypeCustomAttributes(typeof(MaxLengthAttribute), out asType);
                        if (asType)
                        {
                            int length = ((MaxLengthAttribute)objAtrr).Length;
                            colType += "(" + (length < 1 || length > (colType.StartsWith("n") ? 8000 : 4000) ? "max" : length.ToString()) + ")";
                        }
                        else
                        {
                            colType += "(max)";
                        }
                    }
                    return new KeyValuePair<string, string>(property.Name, colType);
                }
            }
            if (entityMapDbColumnType.TryGetValue(property.PropertyType, out string value))
            {
                colType = value;
            }
            else
            {
                colType = SqlDbTypeName.NVarChar;
            }
            if (lenght && colType == SqlDbTypeName.NVarChar)
            {
                colType = "nvarchar(max)";
            }
            return new KeyValuePair<string, string>(property.Name, colType);
        }

        public static string GetKeyName(this Type typeinfo)
        {
            return typeinfo.GetProperties().GetKeyName();
        }
        public static string GetKeyType(this Type typeinfo)
        {
            string keyType = typeinfo.GetProperties().GetKeyName(true);
            if (keyType == "varchar")
            {
                return "varchar(max)";
            }
            else if (keyType != "nvarchar")
            {
                return keyType;
            }
            else
            {
                return "nvarchar(max)";
            }
        }

        /// <summary>
        /// 获取数据库类型，不带长度，如varchar(100),只返回的varchar
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GetSqlDbType(this PropertyInfo propertyInfo)
        {
            string dbType = propertyInfo.GetTypeCustomValue<ColumnAttribute>(x => new { x.TypeName });

            if (string.IsNullOrEmpty(dbType))
            {
                return dbType;
            }
            dbType = dbType.ToLower();
            if (dbType.Contains(SqlDbTypeName.NVarChar))
            {
                dbType = SqlDbTypeName.NVarChar;
            }
            else if (dbType.Contains(SqlDbTypeName.VarChar))
            {
                dbType = SqlDbTypeName.VarChar;
            }
            else if (dbType.Contains(SqlDbTypeName.NChar))
            {
                dbType = SqlDbTypeName.NChar;
            }
            else if (dbType.Contains(SqlDbTypeName.Char))
            {
                dbType = SqlDbTypeName.Char;
            }

            return dbType;
        }


        private static readonly Dictionary<Type, string> entityMapDbColumnType = new Dictionary<Type, string>() {
                    {typeof(int),SqlDbTypeName.Int },
                    {typeof(int?),SqlDbTypeName.Int },
                    {typeof(long),SqlDbTypeName.BigInt },
                    {typeof(long?),SqlDbTypeName.BigInt },
                    {typeof(decimal),"decimal(18, 5)" },
                    {typeof(decimal?),"decimal(18, 5)"  },
                    {typeof(double),"decimal(18, 5)" },
                    {typeof(double?),"decimal(18, 5)" },
                    {typeof(float),"decimal(18, 5)" },
                    {typeof(float?),"decimal(18, 5)" },
                    {typeof(Guid),"UniqueIdentifier" },
                    {typeof(Guid?),"UniqueIdentifier" },
                    {typeof(byte),"tinyint" },
                    {typeof(byte?),"tinyint" },
                    {typeof(string),"nvarchar" }
        };


        public enum FieldType
        {
            VarChar = 0,
            NvarChar,
            Int,
            BigInt,
            UniqueIdentifier
        }
    }
}
