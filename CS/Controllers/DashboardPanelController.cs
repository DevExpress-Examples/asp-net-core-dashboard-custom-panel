using System.Linq;
using System.Threading.Tasks;
using ASPNETCoreDashboardCustomPanel.Code;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreDashboardCustomPanel.Controllers {
    [Route("dashboardpanel")]
    public class DashboardPanelController : Controller {
        private NorthwindContext nwindContext;
        public DashboardPanelController(NorthwindContext nwindContext) {
            this.nwindContext = nwindContext;
        }
        [HttpGet("dashboards")]
        public async Task<IActionResult> Dashboards(DataSourceLoadOptions loadOptions) {
            var source = nwindContext.Products.Select(item => new {
                item.ProductID,
                item.ProductName
            });

            loadOptions.PrimaryKey = new[] { "ProductID" };
            loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(source, loadOptions));
        }
    }
}
