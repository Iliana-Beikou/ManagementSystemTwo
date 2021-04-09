﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ManagementSystemVersionTwo.Models;
using ManagementSystemVersionTwo.Services.Data;
using ManagementSystemVersionTwo.Services.Filtering;
using ManagementSystemVersionTwo.Services.PaypalServices;
using ManagementSystemVersionTwo.Services.Sorting;
using ManagementSystemVersionTwo.Services.SortingAndFiltering;
using ManagementSystemVersionTwo.Services.ViewBags;
using Newtonsoft.Json;
using Paypal;

namespace ManagementSystemVersionTwo.Controllers
{
    public class PaymentController : Controller
    {
        private DataRepository _data;
        private ExternalPaymentServices _external;
        private FillViewBags _fillViewBag;

        public PaymentController()
        {
            _data = new DataRepository();
            _external = new ExternalPaymentServices();
            _fillViewBag = new FillViewBags();
        }

        protected override void Dispose(bool disposing)
        {
            _data.Dispose();
            _external.Dispose();
            _fillViewBag.Dispose();

        }
        // GET: Payment
        public ActionResult Index(string searchName, string orderBy)
        {
            var data = _data.Worker.AllWorkers();

            data = SortingAndFilteringData.SortAndFilteringPayments(searchName, orderBy, data);

            ViewBag.SortSalary = _fillViewBag.SalarySortingOptionsViewBag();

            ViewBag.Names = _fillViewBag.GetWorkerNamesForAutocomplete(data);

            return View(data);
        }
        
        public ActionResult ShowPartial(int id)
        {
            var worker = _data.Worker.FindWorkerByID(id);

            return PartialView("PartialViewForPayment",worker);
        }

        public async Task<ActionResult> MakePayment(int id)
        {
            var worker = _data.Worker.FindWorkerByID(id);

            var statusOfPayment = await Paypal.Paypal.MakePayment(worker.Salary, worker.ApplicationUser.Email);

            if (statusOfPayment is null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _external.SavePayment(statusOfPayment, worker.ID);

                return RedirectToAction("Index");
            }

        }
        public ActionResult WorkerPaymentHistory(string searchName, string orderBy)
        {
            var data = _data.Worker.AllWorkers();

            data = SortingAndFilteringData.SortAndFilteringPayments(searchName, orderBy, data);

            ViewBag.SortSalary = _fillViewBag.SalarySortingOptionsViewBag();

            ViewBag.Names = _fillViewBag.GetWorkerNamesForAutocomplete(data);

            return View(data);
        }

    }
}