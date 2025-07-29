using fd.infrastructure.entity.SysModels;
using fd.infrastructure.entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace fd.infrastructure.core.Controller
{

    public class ApiBaseController<TService> : ControllerBase
    {
        protected readonly ILogger _logger;
        protected readonly TService _service;
        private WebResponseContent _baseWebResponseContent { get; set; }

        public ApiBaseController()
        {
        }
        public ApiBaseController(TService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        //[ActionLog("查询")]
        //[ApiActionPermission(Enums.ActionPermissionOptions.Search)]               
        [HttpGet, HttpPost, Route("GetPageData")]
        public virtual ActionResult GetPageData(PageDataOptions options)
        {
            var result = InvokeService("GetPageData", new object[] { options });
            WebResponseContent content = new WebResponseContent();

            return Ok(content.OK("", result));
        }

        /// <summary>
        /// 获取明细grid分页数据
        /// </summary>
        /// <param name="loadData"></param>
        /// <returns></returns>
        //[ActionLog("明细查询")]
        //[ApiActionPermission(Enums.ActionPermissionOptions.Search)]
        [HttpPost, Route("GetDetailPage")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ActionResult GetDetailPage([FromBody] PageDataOptions loadData)
        {
            var result = InvokeService("GetDetailPage", new object[] { loadData });
            WebResponseContent content = new WebResponseContent();

            return Ok(content.OK("", result));
        }       

        /// <summary>
        /// 新增支持主子表
        /// </summary>
        /// <param name="saveDataModel"></param>
        /// <returns></returns>
        //[ActionLog("新建")]
        //[ApiActionPermission(Enums.ActionPermissionOptions.Add)]
        [HttpPost, Route("AddRaw")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ActionResult Add([FromBody] SaveModel saveModel)
        {
            _baseWebResponseContent = InvokeService("Add",
                new Type[] { typeof(SaveModel) },
                new object[] { saveModel }) as WebResponseContent;
            return Ok(_baseWebResponseContent);
        }

        /// <summary>
        /// 编辑支持主子表
        /// [ModelBinder(BinderType =(typeof(ModelBinder.BaseModelBinder)))]可指定绑定modelbinder
        /// </summary>
        /// <param name="saveDataModel"></param>
        /// <returns></returns>
        //[ActionLog("编辑")]
        //[ApiActionPermission(Enums.ActionPermissionOptions.Update)]
        [HttpPost, Route("UpdateRaw")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ActionResult Update([FromBody] SaveModel saveModel)
        {
            _baseWebResponseContent = InvokeService("Update", new object[] { saveModel }) as WebResponseContent;
            //Logger.Info(Enums.LoggerType.Edit, null, _baseWebResponseContent.status ? "Ok" : _baseWebResponseContent.message);            
            return Ok(_baseWebResponseContent);
        }

        /// <summary>
        /// 通过key删除文件
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        // [ActionLog("删除")]
        //[ApiActionPermission(Enums.ActionPermissionOptions.Delete)]
        //[HttpPost, Route("Del")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public virtual ActionResult Del([FromBody] object[] keys)
        //{
        //    _baseWebResponseContent = InvokeService("Del", new object[] { keys, true }) as WebResponseContent;
        //    return Ok(_baseWebResponseContent);
        //}
        
        [HttpPost, Route("Del/{id}")]        
        public virtual async Task<ActionResult> Del(int id)
        {
            var task = InvokeService("SoftDelAsync",new object[] { id }) as Task<WebResponseContent>;
            _baseWebResponseContent = await task;
            return Ok(_baseWebResponseContent);
        }

        

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileInput"></param>
        /// <returns></returns>
        //[ActionLog("上传文件")]
        [HttpPost, Route("Upload")]
        //[ApiActionPermission(Enums.ActionPermissionOptions.Upload)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult Upload([FromForm()] IEnumerable<IFormFile> fileInput)
        {
            _baseWebResponseContent = InvokeService("Upload", new object[] { fileInput }) as WebResponseContent;
            return Ok(_baseWebResponseContent);
        }

        /// <summary>
        /// 调用service方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object InvokeService(string methodName, object[] parameters)
        {
            return _service.GetType().GetMethod(methodName).Invoke(_service, parameters);
        }

        /// <summary>
        /// 调用service方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="types">为要调用重载的方法参数类型：new Type[] { typeof(SaveDataModel)</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object InvokeService(string methodName, Type[] types, object[] parameters)
        {
            return _service.GetType().GetMethod(methodName, types).Invoke(_service, parameters);
        }
    }
}