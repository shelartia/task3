using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WithMongo.Models;
using WithMongo.Properties;

namespace WithMongo.Controllers
{
    public class HomeController : Controller
    {
        /*public MongoDatabase MongoDB;

        public HomeController()
        {
            //get the mongo client
            var mongoclient = new MongoClient(Settings.Default.UserInfoesConnetcionString);

            //get the mongo server from the client instance
            var mongoserver = mongoclient.GetServer();

            //Assign the database to MongoDB
            MongoDB = mongoserver.GetDatabase(Settings.Default.DB);
        }
        */
        public UserInfoRepository Context = new UserInfoRepository();

        public ActionResult Index()
        {


            return View("Index", Context.GetAllUserInfoes());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.FirstName = Request.Form["FirstName"];
            userInfo.LastName = Request.Form["LastName"];
            userInfo.Address = Request.Form["Address"];
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/images/profile"), pic);
                // file is uploaded
                file.SaveAs(path);


                // save the image  byte[] for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    userInfo.Photo = ms.GetBuffer();
                }

                if (ModelState.IsValid)
                {
                    Context.Add(userInfo);
                    
                    return RedirectToAction("Index");
                }

            }
            // model was not saved and redirect again to create
            return RedirectToAction("Create", userInfo);
        }

        public ActionResult Delete(string id = "")
        {
            UserInfo model = Context.GetUserInfoById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            
            Context.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
