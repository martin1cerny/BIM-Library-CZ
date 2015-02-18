using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLWebSpec.Controllers
{
    public class ProjectInformationController : Controller
    {
        //
        // GET: /ProjectInformation/

        public ActionResult Index()
        {
            var model = new BLData.BLModel();
            return View(model.Information);
        }

        //
        // GET: /ProjectInformation/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ProjectInformation/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ProjectInformation/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ProjectInformation/Edit/5

        public ActionResult Edit()
        {
            return View();
        }

        //
        // POST: /ProjectInformation/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ProjectInformation/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ProjectInformation/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
