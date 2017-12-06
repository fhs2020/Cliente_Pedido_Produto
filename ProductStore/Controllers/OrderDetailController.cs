using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProductStore.Models;
using System.Net.Mail;
using Microsoft.AspNet.Identity;

namespace ProductStore.Controllers
{
    [Authorize]
    public class OrderDetailController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderDetail
        public ActionResult Index()
        {
            // var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            // return View(orderDetails.ToList());

            return View();
        }

        // GET: OrderDetail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: OrderDetail/Create
        public ActionResult Create(int? ID)
        {
            var id = RouteData.Values["id"];

            var pedidoId = Convert.ToInt32(id);

            var pedidos = db.Orders.Find(pedidoId);


            if (pedidos.PagamentoBoleto == true)
            {
                ViewBag.Boleto = true;
            }
            else
            {
                ViewBag.Boleto = false;
            }

            if (pedidos.PagamentoCartaoCredito == true)
            {
                ViewBag.Credito = true;
            }
            else
            {
                ViewBag.Credito = false;
            }

            if (pedidos.PagamentoCheque == true)
            {
                ViewBag.Cheque = true;
            }
            else
            {
                ViewBag.Cheque = false;
            }


            var orderDetails = new OrderDetail();

            orderDetails.OrderId = pedidoId;
            ViewBag.CustomerName = pedidos.Customer;


            var listaProduto = db.Products.AsEnumerable().Select(c => new
            {
                Id = c.Id,
                NomeProduto = string.Format("{0} - {1}", c.Nome, c.Preco)
            }).ToList();


            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Customer", orderDetails);
            ViewBag.ProductId = new SelectList(listaProduto, "Id", "NomeProduto");

            return View(orderDetails);
        }

        // POST: OrderDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                var produto = db.Products.Find(orderDetail.ProductId);

                var valTotal = (produto.Preco * orderDetail.Quantity);

                orderDetail.Total = valTotal;

                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Customer", orderDetail.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Nome", orderDetail.ProductId);
            return View(orderDetail);
        }


        public bool UpdateProduto(int produtoId, int quantidade)
        {
            var produto = db.Products.Find(produtoId);

            var qtyEmEstoque = (produto.Quantidade - quantidade);

            if (qtyEmEstoque > 0)
            {
                produto.Quantidade = qtyEmEstoque;

                produto.DisponivelEmEstoque = true;

                db.Entry(produto).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            else
            {
                produto.DisponivelEmEstoque = false;

                return false;

            }
        }

        [HttpPost]
        public string PostSendGmail(int id, bool pagamentoBoleto, bool pagamentoCheque, bool pagamentoCartao)
        {
            var order = db.Orders.Find(id);

            order.PagamentoBoleto = pagamentoBoleto;
            order.PagamentoCheque = pagamentoCheque;
            order.PagamentoCartaoCredito = pagamentoCartao;

            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            if (order.PagamentoBoleto == true)
            {
                ViewBag.Boleto = true;
            }
            else
            {
                ViewBag.Boleto = false;
            }

            if (order.PagamentoCartaoCredito == true)
            {
                ViewBag.Credito = true;
            }
            else
            {
                ViewBag.Credito = false;
            }

            if (order.PagamentoCheque == true)
            {
                ViewBag.Cheque = true;
            }
            else
            {
                ViewBag.Cheque = false;
            }

            var usuarioId = User.Identity.GetUserId();
       

            var usuario = db.Users.Find(usuarioId);

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("flavio.ti@repnovageracao.com.br", "baller100");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            //can be obtained from your model
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("ruraltech@repnovageracao.com.br");
            msg.To.Add(new MailAddress("ruraltech@repnovageracao.com.br"));
            msg.To.Add(new MailAddress(usuario.UserName));

            msg.Subject = "Pedido Enviado com sucesso";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body><b>Pedido número " + order.Id + " foi enviado com sucesso por " + usuario.UserName + " </b></body>");
            try
            {
                client.Send(msg);
                return "OK";
            }
            catch (Exception ex)
            {

                return "error:" + ex.ToString();
            }
        }


        // GET: OrderDetail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Customer", orderDetail.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Nome", orderDetail.ProductId);
            return View(orderDetail);
        }


        [HttpPost]
        public ActionResult Edit(int pedidoID, int produtoID, int quantidade)
        {

            var orderDetail = new OrderDetail();

            var pedido = db.Orders.Find(pedidoID);
            orderDetail.OrderId = pedidoID;

            var produto = db.Products.Find(produtoID);
            orderDetail.ProductId = produtoID;

            var emEstoque = UpdateProduto(produtoID, quantidade);

            if (emEstoque == false)
            {
                var resultado = new { Success = "False", Message = "Produto fora de estoque ou quandidade do pedido esta maior que a quandidade dísponivel em nosso estoque!" };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }


            var listaItems = db.OrderDetails.Where(x => x.OrderId == pedidoID).ToList();

            var items = listaItems.Where(m => m.ProductId == produtoID).ToList();

            if (items != null && items.Count > 0)
            {
                foreach(var item in items)
                {
                    item.Quantity += quantidade;

                    item.Total = (item.Quantity * item.ValorProdudo).Value;


                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                }


            }
            else
            {
                orderDetail.Quantity = quantidade;

                orderDetail.ValorProdudo = produto.Preco;

                orderDetail.Total = (orderDetail.Quantity * produto.Preco);

                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }



            //ViewBag.OrderId = new SelectList(db.Orders, "Id", "Customer", orderDetail.OrderId);
            //ViewBag.ProductId = new SelectList(db.Products, "Id", "Nome", orderDetail.ProductId);

            var result = new { Success = "True", Message = "Produto adicionado com sucesso!" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCartItems(int? pedidoId)
        {
            //string x = Request.QueryString["id"];

            //var queryString = HttpContext.Request.QueryString.Get("id");

            //var idPedido = Convert.ToInt32(x);

            decimal sum = 0;

            var GetItems = new List<OrderDetail>();

            if (GetItems != null)
            {
                GetItems = db.OrderDetails.Where(s => s.OrderId == pedidoId.Value).ToList();

              
            }

            var orderDetails = new List<OrderDetail>();

            if (GetItems != null)
            {
                foreach(var item in GetItems)
                {
                    var produto = db.Products.Find(item.ProductId);
                    var order = db.Orders.Find(pedidoId.Value);

                    item.ProdutoNome = produto.Nome;
                    item.ValorProdudo = produto.Preco;

                    if (produto.Nome == "Sensor")
                    {
                        item.SensorBasico = true;


                    }
                    else if (produto.Nome.Contains("Premium"))
                    {
                        item.SensorPremium = true;
                    }
                   

                    orderDetails.Add(item);

                }
            }

            if (orderDetails != null)
            {
                foreach (var totalsum in orderDetails)
                {
                    sum = sum + totalsum.Total;
                }

            }

            var pedido = db.Orders.Find(pedidoId);

            if (pedido.PagamentoBoleto == true)
            {
                ViewBag.Boleto = true;
            }
            else
            {
                ViewBag.Boleto = false;
            }

            if (pedido.PagamentoCartaoCredito == true)
            {
                ViewBag.Credito = true;
            }
            else
            {
                ViewBag.Credito = false;
            }

            if (pedido.PagamentoCheque == true)
            {
                ViewBag.Cheque = true;
            }
            else
            {
                ViewBag.Cheque = false;
            }


            ViewBag.ValorTotal = sum;

            ViewBag.ListaOrcamentos = orderDetails;


            var itemsGrupado = orderDetails.GroupBy(x => x.ProductId).Select(x => x).ToList();


            //"<tr id='" + ListaItems[i].ID + "'>" +
            //                    "<td>" + ListaItems[i].ProdutoNome + "</td>" +
            //                    "<td>" + ListaItems[i].Quantity + "</td>" +
            //                    "<td> R$ " + ListaItems[i].ValorProdudo + "</td>" +
            //                    "<td> R$ " + ListaItems[i].Total + "</td>" +

 


            return Json(orderDetails);
        }


        // GET: OrderDetail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetail/Delete/5
        [HttpPost, ActionName("Excluir")]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();

            //var result = new { Success = "True", Message = "Item foi excluido com sucesso!" };

            return Json(new { excluido = orderDetail.OrderId });
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
