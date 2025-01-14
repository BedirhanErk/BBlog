﻿using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBlog.UI.Controllers
{
    public class BlogController : Controller
    {
        BusinessLayer.Concrete.UserManager um = new BusinessLayer.Concrete.UserManager(new EfUserRepository());
        BlogManager bm = new BlogManager(new EfBlogRepository());
        CategoryManager cm = new CategoryManager(new EfCategoryRepository());
        private readonly UserManager<AppUser> _userManager;
        public BlogController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public class ParamBlogId
        {
            public int id { get; set; }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            var values = bm.GetBlogListWithCategory().Where(x => x.Status == true).ToList();
            return View(values);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index(string blog)
        {
            List<Blog> values = new List<Blog>();
  
            values = bm.GetBlogListWithCategory().Where(x => x.Title.ToLower().Trim().Contains(blog.ToLower().Trim()) || x.BlogContent.ToLower().Trim().Contains(blog.ToLower().Trim()) || x.Category.Name.ToLower().Trim().Contains(blog.ToLower().Trim())).ToList();

            return View(values);
        }
        [AllowAnonymous]
        public IActionResult BlogDetails(int id)
        {
            ViewBag.id = id;
            ViewBag.comment = bm.GetBlogCommentCount(id);
            var values = bm.GetById(id);
            var writer = um.GetById(values.WriterId);
            ViewBag.author = writer.Name + " " + writer.Surname;
            return View(values);
        }
        public async Task<IActionResult> BlogListByWriter()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var blogs = bm.GetBlogListWithCategoryByWriter(user.Id);
            return View(blogs);
        }
        [HttpGet]
        public IActionResult AddBlog()
        {
            List<SelectListItem> categories = (from x in cm.GetAll()
                                               select new SelectListItem
                                               {
                                                   Text = x.Name,
                                                   Value = x.CategoryId.ToString()
                                               }).ToList();
            ViewBag.c = categories;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddBlog(Blog blog)
        {
            BlogValidator bv = new BlogValidator();
            ValidationResult result = bv.Validate(blog);
            if (result.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                blog.Status = true;
                blog.CreDate = DateTime.Now;
                blog.WriterId = user.Id;
                bm.Add(blog);
                return RedirectToAction("BlogListByWriter", "Blog");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
            }
            return View();
        }
        public IActionResult DeleteBlog([FromBody]ParamBlogId request)
        {
            var blog = bm.GetById(request.id);
            blog.Status = (blog.Status == false) ? true : false;
            bm.Update(blog);
            return Json(blog);
        }
        [HttpGet]
        public IActionResult UpdateBlog(int id)
        {
            List<SelectListItem> categories = (from x in cm.GetAll()
                                               select new SelectListItem
                                               {
                                                   Text = x.Name,
                                                   Value = x.CategoryId.ToString()
                                               }).ToList();

            ViewBag.c = categories;
            var blog = bm.GetById(id);
            return View(blog);
        }
        [HttpPost]
        public IActionResult UpdateBlog(Blog blog)
        {
            var nowBlog = bm.GetById(blog.BlogId);
            blog.Status = nowBlog.Status;
            blog.WriterId = nowBlog.WriterId;
            blog.BlogRatingId = nowBlog.BlogRatingId;
            blog.CreDate = nowBlog.CreDate;
            bm.Update(blog);
            return RedirectToAction("BlogListByWriter");
        }
    }
}
