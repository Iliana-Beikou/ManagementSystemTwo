﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManagementSystemVersionTwo.Models;
using ManagementSystemVersionTwo.Services.Data;
using ManagementSystemVersionTwo.Services.DepartmentServices;
using Microsoft.AspNet.Identity;

namespace ManagementSystemVersionTwo.Controllers
{
    public class DepartmentController : Controller
    {
        private CRUDDepartment _crud;
        private DataRepository _data;

        public DepartmentController()
        {
            _crud = new CRUDDepartment();
            _data = new DataRepository();
        }

        protected override void Dispose(bool disposing)
        {
            _crud.Dispose();
            _data.Dispose();
        }

        public ActionResult CreateDepartment()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                _crud.AddDepartment(department);
                return RedirectToAction("ViewAllDepartments", "Display");
            }
            return View();
        }

        public ActionResult EditDepartment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var department = _data.FindDepartmentByID((int)id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                _crud.EditDepartment(department);
                return RedirectToAction("ViewAllDepartments", "Display");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult DeleteDepartment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var department = _data.FindDepartmentByID((int)id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDepartment(int id)
        {
            var department = _data.FindDepartmentByID(id);
            _crud.DeleteDepartment(department);
            return RedirectToAction("ViewAllDepartments", "Display");
        }


        public ActionResult Chat()
        {
            var id = User.Identity.GetUserId();
            var user = _data.FindUserByID(id);
            var messages = user.Worker.Department.Messages.ToList();
            ViewBag.Name = user.Worker.FullName;
            return View(messages);
        }


    }
}