using System.Linq;
using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
    [ODataRoutePrefix("Person")]
    public class PersonController : ODataController
    {
        private AdventureWorks2012Context _db;

        public PersonController(AdventureWorks2012Context AdventureWorks2016Context)
        {
            _db = AdventureWorks2016Context;
        }

        [ODataRoute]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IActionResult Get()
        {
            return Ok(_db.Person.AsQueryable());
        }

        [ODataRoute("({key})")]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IActionResult Get([FromODataUri] int key)
        {
            return Ok(_db.Person.Find(key));
        }

        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        [ODataRoute("Default.MyFirstFunction")]
        [HttpGet]
        public IActionResult MyFirstFunction()
        {
            return Ok(_db.Person.Where(t => t.FirstName.StartsWith("K")));
        }
    }
}