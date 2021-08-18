using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Restful.Common.Cache;
using Restful.Model;
using Restful.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restful.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestController : ControllerBase
    {
        private readonly TableViewService tableViewService;

        public RestController(TableViewService _tableViewService)
        {
            tableViewService = _tableViewService;
        }

        // GET api/<RestController>/5
        [HttpGet]
        public string Get(string input)
        {
            UrlParameter urlParameter = new UrlParameter();
            urlParameter.TableViewName = "A";
            urlParameter.Columns = "C1,C2,C3,C4,C";
            urlParameter.SortBy = "C1,C2-";
            urlParameter.Parameters = new List<Parameter>();
            urlParameter.Parameters.Add(new Parameter()
            {
                Id = "C1",
                OP = "EQ",
                Value = "1"
            });
            urlParameter.Parameters.Add(new Parameter()
            {
                Id = "C2",
                OP = "EQ",
                Value = "1"
            });
            urlParameter.Args = "SUM=C1,MAX=C2,MIN=C3,AVG=C";
            return JsonConvert.SerializeObject(urlParameter);
            // return JsonConvert.SerializeObject(tableViewService.GetTableViewInfoCache().Where(m => m.TableViewName == "A").Select(m => m));
        }

        [Route("Query")]
        [HttpPost]
        public string Query(string input)
        {
            return tableViewService.GetInstance().GetQueryResult(JsonConvert.DeserializeObject<UrlParameter>(input));
        }

        [Route("Add")]
        [HttpPost]
        public string Add(string input)
        {
            return tableViewService.GetInstance().DoAdd(JsonConvert.DeserializeObject<UrlParameter>(input));
        }

        [Route("Update")]
        [HttpPost]
        public string Update(string input)
        {
            return tableViewService.GetInstance().DoUpdate(JsonConvert.DeserializeObject<UrlParameter>(input));
        }

        [Route("Delete")]
        [HttpPost]
        public string Delete(string input)
        {
            return tableViewService.GetInstance().DoDelete(JsonConvert.DeserializeObject<UrlParameter>(input));
        }


    }
}
