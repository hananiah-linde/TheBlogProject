﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;

namespace TheBlogProject.Controllers;

public class CommentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<BlogUser> _userManager;

    public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Administrator, Moderator")]
    public async Task<IActionResult> ModeratedIndex()
    {
        var moderatedComments = await _context.Comments
            .Where(c => c.Moderated != null)
            .Include(c => c.Post)
            .Include(c => c.Author)
            .ToListAsync();

        ViewData["MainText"] = "Comments";
        ViewData["SubText"] = "All Moderated Comments";

        return View("Index", moderatedComments);
    }

    [Authorize(Roles = "Administrator, Moderator")]
    public async Task<IActionResult> DeletedIndex()
    {
        var deletedComments = await _context.Comments
            .Where(c => c.Deleted != null)
            .Include(c => c.Post)
            .Include(c => c.Author)
            .ToListAsync();

        ViewData["MainText"] = "Comments";
        ViewData["SubText"] = "All Deleted Comments";
        ViewData["HardDelete"] = "true";

        return View("Index", deletedComments);
    }

    //GET: Comments
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Index()
    {

        var allComments = await _context.Comments
            .Include(p => p.Post)
            .Include(p => p.Author)
            .Include(p => p.Moderator)
            .Where(c => c.Moderated == null)
            .OrderByDescending(c => c.Created)
            .ToListAsync();

        ViewData["MainText"] = "Comments";
        ViewData["SubText"] = "All Unmoderated Comments";

        return View(allComments);
    }

    // POST: Comments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PostId,Body")] Comment comment)
    {
        if (ModelState.IsValid)
        {
            comment.AuthorId = _userManager.GetUserId(User);
            comment.Created = DateTime.Now;

            _context.Add(comment);
            await _context.SaveChangesAsync();

            comment = await _context.Comments
                .Include(c => c.Post)
                .Where(c => c.Id == comment.Id)
                .FirstOrDefaultAsync();

            if (comment is null) return NotFound();

            return RedirectToAction("Details", "Posts", new { Slug = comment.Post.Slug }, "commentSection");
        }

        return View(comment);
    }

    // GET: Comments/Edit/5
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var comment = await _context.Comments
            .Include(c => c.Author)
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (comment == null) return NotFound();

        ViewData["ModerationType"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);

        return View(comment);
    }

    // POST: Comments/Edit/5
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,Body")] Comment comment)
    //{
    //    if (id != comment.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
    //        try
    //        {
    //            newComment.Body = comment.Body;
    //            newComment.Updated = DateTime.Now;

    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!CommentExists(comment.Id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction("Details", "Posts", new {slug = newComment.Post.Slug}, "commentSection");
    //    }

    //    return View(comment);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Moderate(int id, [Bind("Id,Body,ModeratedBody,ModerationType")] Comment comment)
    {
        if (id != comment.Id) return NotFound();

        var commentDb = await _context.Comments
            .Include(c => c.Post)
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == comment.Id);
        if (ModelState.IsValid)
        {

            try
            {
                commentDb.ModeratedBody = comment.ModeratedBody;
                commentDb.ModerationType = comment.ModerationType;
                commentDb.Moderated = DateTime.Now;
                commentDb.ModeratorId = _userManager.GetUserId(User);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", "Posts", new { slug = commentDb.Post.Slug }, "commentSection");
        }
        comment.Author = commentDb.Author;
        return View("Edit", comment);
    }

    // GET: Comments/Delete/5
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var comment = await _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Moderator)
            .Include(c => c.Post)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (comment is null) return NotFound();

        return View(comment);
    }

    // POST: Comments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string slug, string hardDelete)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (hardDelete == "true")
        {
            _context.Comments.Remove(comment);
        }
        else
        {
            comment.Deleted = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Posts", new { slug }, "commentSection");
    }

    private bool CommentExists(int id)
    {
        return _context.Comments.Any(e => e.Id == id);
    }
}
