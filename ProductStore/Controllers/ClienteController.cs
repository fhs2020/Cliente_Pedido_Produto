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
    public class ClienteController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cliente
        public ActionResult Index(String search)
        {
            var usuarioId = User.Identity.GetUserId();

            var usuario = db.Users.Find(usuarioId);

            if (!String.IsNullOrEmpty(search))
            {
                var searchWord = search.ToLower();

                var listaClientes = db.Clientes.ToList();

                var clienteSearched = listaClientes.Where(s => s.Nome.ToLower().Contains(searchWord) ||
                           s.VendedorNome.ToLower().Contains(searchWord) ||
                           s.Fazenda.ToLower().Contains(searchWord) ||
                             s.EnderecoRural.ToLower().Contains(searchWord) ||
                               s.Email.ToLower().Contains(searchWord) ||
                                 s.CPF.ToLower().Contains(searchWord) ||
                                   s.EnderecoResidencial.ToLower().Contains(searchWord) ||
                                     s.EnderecoRural.ToLower().Contains(searchWord) ||
                                        s.Telefone.ToLower().Contains(searchWord)).ToList();

                if (clienteSearched != null)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return View(clienteSearched);
                    }
                    else
                    {
                        var clientes = clienteSearched.Where(x => x.VendedorNome == usuario.UserName);

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
                return View(db.Clientes.ToList());
            }
            else
            {
                var clientes = db.Clientes.Where(x => x.VendedorNome == usuario.UserName);

                return View(clientes);
            }


        }

        // GET: Cliente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var usuarioId = User.Identity.GetUserId();
                cliente.UserId = usuarioId;

                var usuario = db.Users.Find(usuarioId);

                cliente.VendedorNome = usuario.UserName;

                db.Clientes.Add(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);

            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        [HttpPost]
        public ActionResult Edit(Cliente cliente)
        {

            var usuarioId = User.Identity.GetUserId();

            var usuario = db.Users.Find(usuarioId);

            if (ModelState.IsValid)
            {
                cliente.VendedorNome = usuario.UserName;

                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);

            var pedidos = db.Orders.ToList();

            if (pedidos != null)
            {
                foreach (var pedido in pedidos)
                {
                    if (pedido.ClienteID == id)
                    {
                        //return Json(new { success = false, responseText = "Cliente não pode ser excluido! Existe um pedido associado a este cliente." }, JsonRequestBehavior.AllowGet);

                        ViewBag.Error = "Cliente não pode ser excluido! Existe um pedido associado a este cliente";
                        return View(cliente);
                    }
                }
            }


            db.Clientes.Remove(cliente);
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
