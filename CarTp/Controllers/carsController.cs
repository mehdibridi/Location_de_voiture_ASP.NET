using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CarTp.Models;
using Microsoft.AspNet.Identity;

namespace CarTp.Controllers
{
    public class carsController : Controller
    {
        private Database1Entities4 db = new Database1Entities4();
        private ApplicationDbContext db2 = new ApplicationDbContext();

        // GET: cars
        public ActionResult Index()
        {
            var car = db.car.Include(c => c.client);
            return View(car.ToList());
        }

        // GET: cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            car car = db.car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (car.photo != null) {


                    car.photo = "data:image/"
                        + Path.GetExtension(car.photo).Replace(".", "")
                        + ";base64,"
                        + Convert.ToBase64String(System.IO.File.ReadAllBytes(car.photo));


                }
            }
            return View(car);
        }

        // GET: cars/Create
        public ActionResult Create()
        {
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom");
            return View();
        }

        // POST: cars/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCar,marque,kilometrage,prix,idClient,photo")] car car)
        {
            car.ImageFile = Request.Files[0];

            if (ModelState.IsValid)
            {

                string FileName = Path.GetFileNameWithoutExtension(car.ImageFile.FileName);

                string FileExtension = Path.GetExtension(car.ImageFile.FileName);

                FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + FileName.Trim() + FileExtension;

                string UploadPath = "D:/4eme anne/semestre 2/dot.net/CarTp/CarTp/image";

                car.photo = UploadPath + FileName;

                car.ImageFile.SaveAs(car.photo);



                db.car.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom", car.idClient);
            return View(car);
        }

        // GET: cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            car car = db.car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom", car.idClient);
            return View(car);
        }

        // POST: cars/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCar,marque,kilometrage,prix,idClient,photo")] car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom", car.idClient);
            return View(car);
        }

        // GET: cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            car car = db.car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            car car = db.car.Find(id);
            db.car.Remove(car);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult HomeClient()
        {

            var car = db.car.Include(c => c.client);
            return View(car.ToList());
        }
        public ActionResult addPanier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            car car = db.car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login","Account");
            }
            else if (User.Identity.IsAuthenticated)
            {
                if (car == null)
                {
                    return HttpNotFound();
                }
                else {
                    client client = db.client.Find(User.Identity.GetUserId());
                    if (car.idClient == null)
                    {
                        car.idClient = client.IdClient;
                        db.Entry(car).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Panier");
                    }
                   
                }

            }
            return View(car);
        }
        public ActionResult Panier()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (User.Identity.IsAuthenticated)
            {
                client client = db.client.Find(User.Identity.GetUserId());

                if (client != null) {

                    
                    return View(db.car.Where(model => model.idClient == client.IdClient).ToList());
                } 
            }
        
            return View();

        }
        public ActionResult removePanier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            car car = db.car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            else if (car.idClient != null)
            {
                car.idClient = null;
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("Panier");
        }

    }
}
