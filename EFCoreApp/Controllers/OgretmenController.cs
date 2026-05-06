using EFCoreApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreApp.Controllers
{
    public class OgretmenController : Controller
    {
        private readonly DataContext _Context;
        public OgretmenController(DataContext context)
        {
            _Context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _Context.Ogretmenler.ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ogretmen model)
        {
            _Context.Ogretmenler.Add(model);
            await _Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var ogretmen = await _Context.Ogretmenler.FirstOrDefaultAsync(m => m.OgretmenId == id);
            if(ogretmen == null)
            {
                return NotFound();
            }
            return View(ogretmen);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ogretmen model)
        {
            if(id != model.OgretmenId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _Context.Update(model);
                    await _Context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if(!_Context.Ogretmenler.Any(m => m.OgretmenId == model.OgretmenId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var ogretmen = await _Context.Ogretmenler
                           .Include(o => o.Kurslar)
                           .FirstOrDefaultAsync(o => o.OgretmenId == id);
            if(ogretmen == null)
            {
                return NotFound();
            }
            if(ogretmen.Kurslar.Count() > 0)
            {
                return View("OgretmenError", ogretmen);
            }
            return View(ogretmen);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm]int ogretmenId)
        {
            var ogretmen = await _Context.Ogretmenler.FindAsync(ogretmenId);
            if(ogretmen == null)
            {
                return NotFound();
            }
            _Context.Remove(ogretmen);
            await _Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
