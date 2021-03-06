﻿using AzR.Web.Controllers;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AzR.Core.HelperModels;
using AzR.Core.Services.Interface;
using AzR.Core.ViewModels.Admin;

namespace AzR.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class UserPrivilegeController : BaseController
    {
        private readonly IUserManager _user;
        private readonly IUserPrivilegeManager _userPrivilege;
        private readonly IMenuManager _menu;
        public UserPrivilegeController(IUserManager user, IUserPrivilegeManager userPrivilege, IMenuManager menu, IBaseManager general) : base(general)
        {
            _user = user;
            _userPrivilege = userPrivilege;
            _menu = menu;
        }

        public ActionResult Index()
        {

            var model = _user.LoadUsers();
            model.Remove(model.SingleOrDefault(u => u.Value == User.Identity.GetUserId()));

            ViewData["lstUser"] = model;

            return PartialView(new SelectDropDownItem());
        }

        [HttpGet]
        public ActionResult AvailablePermission(SelectDropDownItem model)
        {
            var finalList = new List<UserPrivilegeViewModel>();
            if (model.Id == 0) return PartialView(finalList);
            var lstUserPrivilege = _userPrivilege.GetUserwisePrivilages(model.Id);
            finalList = (from up in lstUserPrivilege
                         join menu in _menu.GetAllAsync().Where(m => m.IsActive)
                         on up.MenuId equals menu.Id
                         select up
            ).ToList();
            return PartialView(finalList);
        }

        [HttpPost]
        public ActionResult AddEditPrivilege(List<UserPrivilegeViewModel> model)
        {
            foreach (var up in model)
            {
                if (up.Id == 0)
                {
                    _userPrivilege.Create(up);
                }
                else
                {
                    _userPrivilege.Update(up);
                }
            }

            return
                   Json(
                    new
                    {
                        redirectTo = Url.Action("Index", "UserPrivilege", new { Area = "Admin" }),
                        message = "Record updated Successfully !!!",
                        position = "mainContainer"
                    });
        }

    }
}