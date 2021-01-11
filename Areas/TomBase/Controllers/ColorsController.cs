using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BasePackageModule1.Areas.TomBase.ViewModels;
using BasePackageModule1.Props;
using BasePackageModule2.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SharpScss;
using TomBase.Helpers;

namespace TomBase.Areas.Controllers
{
    [Area("TomBase")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ColorsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IWritableOptions<WebsiteSettings> _writableLocations;


        public ColorsController(IWebHostEnvironment env, IConfiguration configuration, IWritableOptions<WebsiteSettings> writableLocations)
        {
            _env = env;
            _configuration = configuration;
            _writableLocations = writableLocations;
        }

        public IActionResult Index()
        {
            var colors = new Colors
            {
                PrimaryColor = _configuration.GetSection("WebsiteSettings").GetSection("PrimaryColor").Value
            };
            return View(colors);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Colors colors)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _env.WebRootPath;

                var AppScss = $"{webRootPath}/css/app.scss";

                var appCustomScss = $"{webRootPath}/css/custom.scss";

                var OutputScss = $"{webRootPath}/css/app.css";


                await System.IO.File.WriteAllTextAsync(appCustomScss, (await System.IO.File.ReadAllTextAsync(AppScss)).Replace("#f07d00", $"{colors.PrimaryColor}"));

                var result = Scss.ConvertFileToCss(appCustomScss);
                //return Content(result.Css);

                await System.IO.File.WriteAllTextAsync(OutputScss, result.Css, Encoding.UTF8);

                _writableLocations.Update(opt => {
                    opt.PrimaryColor = colors.PrimaryColor;
                });


                return RedirectToAction("Index").WithSuccess("Colors Updated successfully!", "");
            }
            return View("Index", colors).WithError("Incorrect input!", "");

        }

        public async Task<IActionResult> Reset()
        {
            var primaryColor = _configuration.GetSection("WebsiteSettings").GetSection("PrimaryColor").Value;

            var defaultPrimaryColor = _configuration.GetSection("DefaultWebsiteSettings").GetSection("PrimaryColor").Value;


            string webRootPath = _env.WebRootPath;

            var AppScss = $"{webRootPath}/css/app.scss";

            var appCustomScss = $"{webRootPath}/css/custom.scss";

            var OutputScss = $"{webRootPath}/css/app.css";


            await System.IO.File.WriteAllTextAsync(appCustomScss, (await System.IO.File.ReadAllTextAsync(AppScss)).Replace(primaryColor, $"{defaultPrimaryColor}"));

            var result = Scss.ConvertFileToCss(appCustomScss);

            await System.IO.File.WriteAllTextAsync(OutputScss, result.Css, Encoding.UTF8);

            _writableLocations.Update(opt => {
                opt.PrimaryColor = defaultPrimaryColor;
            });


            return RedirectToAction("Index").WithSuccess("Colors have been successfully reset!");
        }
    }
}
