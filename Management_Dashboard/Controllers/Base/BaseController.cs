using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Dashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController<T> : ControllerBase where T : ManagementDashboardEntityBase, new()
    {
        protected readonly BaseService<T> service;

        public BaseController(BaseService<T> baseService)
        {
            service = baseService;
        }

        #region Default Routes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            var result = await service.GetAll();
            return result.ToList();
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> AddOrUpdate(T model)
        {
            var result = await service.Insert(model);
            return result;
        }

        [HttpPost]
        public virtual async Task<ActionResult<IEnumerable<T>>> AddMany(List<T> model)
        {
            var result = await service.InsertMany(model);
            return result.ToList();
        }

        [HttpGet]
        public async Task<ActionResult<T>> Get(Guid id)
        {
            var result = await service.Find(id);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<T>> Delete(Guid id)
        {
            //Load the Data which will be deleted
            var data = await service.Find(id);

            //TODO: Move to Archive Collection
            await service.Delete(id);

            return data;
        }
        #endregion
    }
}

