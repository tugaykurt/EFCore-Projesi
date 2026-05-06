using EFCoreApp.Data;
using EFCoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EFCoreApp.Controllers
{
    public class KursController : Controller
    {
        private readonly DataContext _Context;
        public KursController(DataContext context)
        {
            _Context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _Context.Kurslar.Include(k => k.Ogretmen).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Ogretmenler = new SelectList(await _Context.Ogretmenler.ToListAsync(), "OgretmenId", "AdSoyad");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KursViewModel model)
        {
            if (ModelState.IsValid)
            {
                _Context.Kurslar.Add(new Kurs
                {
                    KursId = model.KursId,
                    Baslik = model.Baslik,
                    OgretmenId = model.OgretmenId
                });
                await _Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var kurs = await _Context
                            .Kurslar
                            .Include(k => k.KursKayilari)
                            .ThenInclude(k => k.Ogrenci)
                            .Select(k => new KursViewModel
                            {
                                KursId = k.KursId,
                                Baslik = k.Baslik,
                                OgretmenId = k.OgretmenId,
                                KursKayilari = k.KursKayilari
                            })
                            .FirstOrDefaultAsync(k => k.KursId == id);
            // Yukarıda şunu yaptık, Kurs modelinde olan, Ogretmen property'sini, ModelState.IsValid kontrolü valid olarak görmediği için bir Dto kullandık. Yani aracı bir view model kullandık. Bunuda aldığımız kurs bilgilerini bu view model type'ıda göstermek için select adında bir metodumuz var. Bu metot sayesinde database'den aldığımız kurs bilgileri, Kurs modeli üzerinde gelip, burada select metodu sayesinde parametresinde KursViewModel view modelini new'leyerek, bu gelen data'yı bu view model type'ında göstermiş olduk.

            if (kurs == null)
            {
                return NotFound();
            }
            ViewBag.Ogretmenler = new SelectList(await _Context.Ogretmenler.ToListAsync(), "OgretmenId", "AdSoyad");
            return View(kurs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, KursViewModel model)
        {
            if (id != model.KursId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _Context.Update(new Kurs()
                    {
                        KursId = model.KursId,
                        Baslik = model.Baslik,
                        OgretmenId = model.OgretmenId
                    }
                    );
                    await _Context.SaveChangesAsync();
                }
                catch (DbUpdateException) // Bu biraz daha genel bir update kontrolü yapıyor.
                {
                    if (!_Context.Kurslar.Any(m => m.KursId == model.KursId))
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
            ViewBag.Ogretmenler = new SelectList(await _Context.Ogretmenler.ToListAsync(), "OgretmenId", "AdSoyad");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var kurs = await _Context.Kurslar.FindAsync(id);
            if (kurs == null)
            {
                return NotFound();
            }
            return View(kurs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int kursId)
        {
            var kurs = await _Context.Kurslar.FindAsync(kursId);
            if (kurs == null)
            {
                return NotFound();
            }
            _Context.Remove(kurs);
            var kursKayit = await _Context.KursKayitlari.Where(k => k.KursId == kursId).ToListAsync();
            if(kursKayit == null)
            {
                return NotFound();
            }
            foreach (var item in kursKayit)
            {
                _Context.Remove(item);
            }
            await _Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
