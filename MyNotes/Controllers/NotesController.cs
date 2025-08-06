using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyNotes.Models;

namespace MyNotes.Controllers
{
    public class NotesController : Controller
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NoteModels
        public async Task<IActionResult> Index()
        {
            var model = new NotesModel(_context);
            model.GetNotes();
            return View("List", model);
        }

        // GET: NoteModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noteModel = await _context.Notes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (noteModel == null)
            {
                return NotFound();
            }

            return View(noteModel);
        }

        public IActionResult List()
        {
            var model = new NotesModel(_context);
            model.GetNotes();
            return View(model);
        }

        // GET: NoteModels/Create
        public IActionResult Create()
        {
            var model = new Note
            {
                CreatedDate = DateTime.Now,

            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            var model = new NotesModel(_context);
            List<Note> results;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                results = model.SearchNotes(searchTerm);
            }
            else
            {
                results = model.GetNotes();
            }
            return PartialView("_SearchResults", results);
        }

        public async Task<IActionResult> Search1(string searchTerm) { 
            var model = new NotesModel(_context);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchNotes(searchTerm);
            }
            else
            {
                model.GetNotes();
            }
            return View("List", model);
        }


        // POST: NoteModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,links,Tags,CreatedDate,UpdatedDate")] Note noteModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(noteModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return RedirectToAction(nameof(List));
        }

        // GET: NoteModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noteModel = await _context.Notes.FindAsync(id);
            if (noteModel == null)
            {
                return NotFound();
            }
            return View(noteModel);
        }

        // POST: NoteModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,links,Tags,CreatedDate,UpdatedDate")] Note noteModel)
        {
            if (id != noteModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(noteModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteModelExists(noteModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var model = new NotesModel(_context);
                model.GetNotes();

                return View("List", model);
            }
            return View(noteModel);
        }

        // GET: NoteModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noteModel = await _context.Notes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (noteModel == null)
            {
                return NotFound();
            }

            return View(noteModel);
        }

        // POST: NoteModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noteModel = await _context.Notes.FindAsync(id);
            if (noteModel != null)
            {
                _context.Notes.Remove(noteModel);
            }

            await _context.SaveChangesAsync();

            var model = new NotesModel(_context);
            model.GetNotes();

            return View("list", model);
        }

        private bool NoteModelExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}
