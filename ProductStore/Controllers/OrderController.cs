using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProductStore.Models;
using Microsoft.AspNet.Identity;

namespace ProductStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public ActionResult Index(String search)
        {
            var usuarioId = User.Identity.GetUserId();

            var usuario = db.Users.Find(usuarioId);


            if (!String.IsNullOrEmpty(search))
            {
                search.ToLower();

                var listaPedidos = db.Orders.ToList();

                var pedidoSearched = listaPedidos.Where(s => s.NomeConsultor.ToLower().Contains(search) || s.Customer.ToLower().Contains(search)).ToList();

                if (pedidoSearched != null && pedidoSearched.Count() > 0)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return View(pedidoSearched);
                    }
                    else
                    {
                        var clientes = pedidoSearched.Where(x => x.NomeConsultor == usuario.UserName);

                        return View(clientes);
                    }

                }
                else
                {
                    return View();
                }
            }



            if (User.IsInRole("Admin"))
            {
                return View(db.Orders.ToList());
            }
            else
            {
                var clientes = db.Orders.Where(x => x.NomeConsultor == usuario.UserName);

                return View(clientes);
            }
        }

        // GET: Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            var clientes = db.Clientes.ToList();

            //var clienteLista = db.Clientes.AsEnumerable().Select(c => new
            //{
            //    ID = c.ID,
            //    NomeCliente = string.Format("{0} - {1} ", c.Empresa, c.Nome)
            //}).ToList();

            var usuarioId = User.Identity.GetUserId();

            var usuario = db.Users.Find(usuarioId);

            if (User.IsInRole("Admin"))
            {
                ViewBag.Clientes = new SelectList(clientes, "ID", "Nome");
            }
            else
            {
                var clientesDoConsultor = db.Clientes.Where(x => x.VendedorNome == usuario.UserName).ToList();

                ViewBag.Clientes = new SelectList(clientesDoConsultor, "ID", "Nome");
            }

            

            return View();
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(Order order)
        {

            var usuarioId = User.Identity.GetUserId();

            var usuario = db.Users.Find(usuarioId);

            order.NomeConsultor = usuario.UserName;

            var cliente = db.Clientes.Find(order.ClienteID);

                order.Customer = cliente.Nome;

                db.Orders.Add(order);
                db.SaveChanges();
                //return RedirectToAction("Index");


                return RedirectToAction("Create/" + order.Id, "OrderDetail");
           

            //return View(order);
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Customer,ClienteID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
    }
}
