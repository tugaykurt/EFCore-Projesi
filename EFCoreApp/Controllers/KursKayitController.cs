using EFCoreApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EFCoreApp.Controllers
{
    // ******************** Modeller Arasında Çok'a Çok İlişki *****************************
    // Burada Kurs ve Ogrenci modellerini çok'a çok ilişki ile ilişkilendireceğiz. Bunu ilişkiyi de KursKayit model'i üzerinden yöneteceğiz.
    // Peki Çok'a çok ilişki nedir? EfCoreKurulumu.cs dosyasına git... 
    public class KursKayitController : Controller
    {
        private readonly DataContext _Context;
        public KursKayitController(DataContext context)
        {
            _Context = context;
        }

        [HttpGet]
        // KursKayit modeline biz hem Ogrenci modelinin, hemde Kurs modelinin objesini ekledik, ve Index view'inde bu objeler üzerinde öğrenci bilgisini ve kurs bilgisini çağırdık ama bu haldeyken gelmez. Çünkü biz sadece KursKayit modelinin listesini aldık ve view bu listeyi yolladık. Bunun için bizim kullanacağımız bir metot var. Include(Katmak/Dahil Etmek) metodu. Bu metot bu listeye, KursKayit model'i ile hem Ogrenci model'inin, hem de Kurs model'inin ortak datalarını ekleyecek.

        //ASP.NET Core (Entity Framework Core) içerisinde Include ve ThenInclude, ilişkili verileri (navigation properties) tek bir veritabanı sorgusuyla getirmek (Eager Loading) için kullanılır. Include ana ilişkiyi yüklerken, ThenInclude bu yüklenen ilişki üzerindeki alt ilişkilere ulaşmayı sağlar. Performansı artırır ve N+1 sorgu sorununu önler.
        /*
         Include (Birincil İlişki): Ana entity'ye bağlı olan doğrudan ilişkili veriyi getirir.
         Örnek: Bir Post tablosu ile Comment tablosu varsa, Postları getirirken Commentleri de beraberinde getirmek.
         ThenInclude (Alt/Zincirleme İlişki): Include ile getirilen veriye bağlı olan başka bir tabloyu getirir.
         Örnek: Post -> Comment -> Author (Yorumun yazarı) ilişkisi varsa, Postları getir, Commentleri Include et, sonra ThenInclude ile Commentlerin yazarlarını (Author) getir. OgrenciController Edit action'una git... 
                 */
        public async Task<IActionResult> Index()
        {
            var kursKayitlari = await _Context.KursKayitlari
                .Include(x => x.Ogrenci)
                .Include(x => x.Kurs)
                .ToListAsync();
            return View(kursKayitlari);
        }

        [HttpGet]
        // Burada öğrenci listesini ve kurs listesini viewbag aracılığı ile create view'indeki select kutusuna taşıyoruz. Ancak bunlar bir liste olduğu için view'deki select kutusu bunları tam bir liste olarak anlayamaz. Select kutusunun anlayacağı dilden bu listeleri 'SelectList' class'ını new'leyerek parametre olarak bunları ToList ile gönderiyoruz. Bu sayede bu listeleri select kutusuna gönderebiliyoruz. Bu arada bunları view sayfasında da yapabiliyoruz, @ tag'i ile...
        // SelectList sınıfı, ASP.NET MVC ve Core projelerinde HTML <select> (açılır menü) elemanlarını doldurmak için kullanılan, veritabanı veya liste verilerini DropdownListe eşleyen bir sınıftır. Veri kaynağı, değer alanı (Value) ve metin alanı (Text)(Parametreler) belirleyerek kolayca seçenek listeleri oluşturmanızı sağlar.
        public async Task<IActionResult> Create()
        {
            ViewBag.Ogrenciler = new SelectList(await _Context.Ogrenciler.ToListAsync(), "OgrenciId", "AdSoyad");
            ViewBag.Kurslar = new SelectList(await _Context.Kurslar.ToListAsync(), "KursId", "Baslik");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KursKayit model)
        {
            model.KayitTarihi = DateTime.Now;
            _Context.KursKayitlari.Add(model);
            await _Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var kursKayit = await _Context.KursKayitlari.FindAsync(id);
            if (kursKayit == null)
            {
                return NotFound();
            }
            ViewBag.Ogrenciler = new SelectList(await _Context.Ogrenciler.ToListAsync(), "OgrenciId", "AdSoyad");
            ViewBag.Kurslar = new SelectList(await _Context.Kurslar.ToListAsync(), "KursId", "Baslik");
            return View(kursKayit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KursKayit model)
        {
            if (id != model.KayitId)
            {
                return NotFound();
            }
            try
            {
                _Context.Update(model);
                await _Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!_Context.KursKayitlari.Any(m => m.KayitId == model.KayitId))
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
        [HttpGet]
        public async Task<IActionResult> Delete(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }
            var kursKayit = await _Context
                                  .KursKayitlari
                                  .Include(o => o.Ogrenci)
                                  .FirstOrDefaultAsync(m => m.KayitId == id)
                                  ;
            if(kursKayit == null)
            {
                return NotFound();
            }
            return View(kursKayit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm]int KayitId)
        {
            var kursKayit = await _Context.KursKayitlari.FindAsync(KayitId);
            if (kursKayit == null) 
            {
                return NotFound();
            }
            _Context.Remove(kursKayit);
            await _Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
