using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarTp.Models;
using Microsoft.AspNet.Identity;

namespace CarTp.Controllers
{
    public class OffresController : Controller
    {
        private Database1Entities4 db = new Database1Entities4();

        // GET: Offres
        public ActionResult Index()
        {
            var offre = db.Offre.Include(o => o.client);
            return View(offre.ToList());
        }

        // GET: Offres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offre offre = db.Offre.Find(id);
            if (offre == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (offre.photo != null)
                {


                    offre.photo = "data:image/"
                        + Path.GetExtension(offre.photo).Replace(".", "")
                        + ";base64,"
                        + Convert.ToBase64String(System.IO.File.ReadAllBytes(offre.photo));


                }
            }
                return View(offre);
        }

        // GET: Offres/Create
        public ActionResult Create()
        {
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom");
            return View();
        }

        // POST: Offres/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdOffre,kilometrage,marque,prix,photo,idClient")] Offre offre)
        {
            offre.ImageFile = Request.Files[0];
            if (ModelState.IsValid)
            {
                string FileName = Path.GetFileNameWithoutExtension(offre.ImageFile.FileName);

                string FileExtension = Path.GetExtension(offre.ImageFile.FileName);

                FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + FileName.Trim() + FileExtension;

                string UploadPath = "D:/4eme anne/semestre 2/dot.net/CarTp/CarTp/image";

                offre.photo = UploadPath + FileName;

                offre.ImageFile.SaveAs(offre.photo);

                db.Offre.Add(offre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom", offre.idClient);
            return View(offre);
        }

        // GET: Offres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offre offre = db.Offre.Find(id);
            if (offre == null)
            {
                return HttpNotFound();
            }
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom", offre.idClient);
            return View(offre);
        }

        // POST: Offres/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdOffre,kilometrage,marque,prix,photo,idClient")] Offre offre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(offre).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idClient = new SelectList(db.client, "IdClient", "nom", offre.idClient);
            return View(offre);
        }

        // GET: Offres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offre offre = db.Offre.Find(id);
            if (offre == null)
            {
                return HttpNotFound();
            }
            return View(offre);
        }

        // POST: Offres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Offre offre = db.Offre.Find(id);
            db.Offre.Remove(offre);
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

        public ActionResult addCarOffre(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offre offre = db.Offre.Find(id);
            if (offre == null)
            {
                return HttpNotFound();
            }
            car car = new car();
            car.marque = offre.marque;
            car.kilometrage = offre.kilometrage;
            car.prix = offre.prix;
            car.photo = offre.photo;

            db.car.Add(car);
            db.SaveChanges(); 

            
            return RedirectToAction("Index");
        }
        public ActionResult AfficherOffre()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (User.Identity.IsAuthenticated)
            {
                client client = db.client.Find(User.Identity.GetUserId());

                if (client != null)
                {


                    return View(db.Offre.Where(model => model.idClient == client.IdClient).ToList());
                }
            }

            return View();

        }
    }
}
