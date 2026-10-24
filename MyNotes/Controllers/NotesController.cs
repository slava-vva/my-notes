using Microsoft.AspNetCore.Mvc;
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

        // GET: Notes
        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        // GET: Notes/List
        public IActionResult List()
        {
            var model = new NotesModel(_context);
            model.GetNotes();
            return View(model);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Search  (live search and "Show more"; returns the results partial)
        [HttpPost]
        public IActionResult Search(string? searchTerm, int skip = 0)
        {
            var model = new NotesModel(_context);
            model.SearchNotes(searchTerm, skip);
            return PartialView("_SearchResults", model);
        }

        public IActionResult Search1(string? searchTerm)
        {
            var model = new NotesModel(_context);
            model.SearchNotes(searchTerm);
            return View("List", model);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View(new Note());
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,links,Tags")] Note note)
        {
            RequireName(note);
            if (!ModelState.IsValid)
            {
                // Show the form again with the user's input and the validation messages.
                var model = new NotesModel(_context) { InputNote = note };
                model.GetNotes();
                return View("List", model);
            }

            note.CreatedDate = DateTime.Now;
            note.UpdatedDate = note.CreatedDate;
            _context.Add(note);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Note \"{note.Name}\" created.";
            return RedirectToAction(nameof(List));
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,links,Tags")] Note input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            RequireName(input);
            if (!ModelState.IsValid)
            {
                // Keep the stored dates for display, show the user's edits.
                input.CreatedDate = note.CreatedDate;
                input.UpdatedDate = note.UpdatedDate;
                return View(input);
            }

            note.Name = input.Name;
            note.Description = input.Description;
            note.links = input.links;
            note.Tags = input.Tags;
            note.UpdatedDate = DateTime.Now;   // CreatedDate is never changed by an edit

            await _context.SaveChangesAsync();

            TempData["Message"] = $"Note \"{note.Name}\" saved.";
            return RedirectToAction(nameof(List));
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Note \"{note.Name}\" deleted.";
            }

            return RedirectToAction(nameof(List));
        }

        // Validated here rather than with [Required] on the entity, so the database schema stays unchanged.
        private void RequireName(Note note)
        {
            if (string.IsNullOrWhiteSpace(note.Name))
            {
                ModelState.AddModelError(nameof(Note.Name), "Please enter a topic name.");
            }
        }
    }
}
