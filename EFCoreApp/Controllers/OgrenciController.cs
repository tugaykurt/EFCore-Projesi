using EFCoreApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreApp.Controllers
{
    public class OgrenciController : Controller
    {
        private readonly DataContext _Context;

        public OgrenciController(DataContext context)
        {
            _Context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _Context.Ogrenciler.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create() // Eğer action metodumuz bir parametre talep(request) etmiyorsa, talep async karşılanması gerekmiyor. 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Bu attribute bir güvenlik attribute'ü. Şu işe yarıyor: Formu açtığımızda asp.net core bizim için formun içerisine otomatik bir hidden input ekliyor. Bu input'un name:_RequestVerifivationToken value'sunda da globade spesifik bir id veriyor. Bu id'yi vermesinin sebebi formu get edenle post eden aynı kişimi buna bakıyor. Yani başkası bize linke tıklatıp formu post edebilir. Bunu önüne geçmek için bu kontrolü tetikleyen Attribute ValidateAntiForgeryToken. 
        public async Task<IActionResult> Create(Ogrenci model) // Eğer action metodumuz bir parametre talep(request) ediyorsa, talep async karşılanması daha sağlıklı olur.
        {
            _Context.Add(model);
            await _Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        //ASP.NET Core (Entity Framework Core) içerisinde Include ve ThenInclude, ilişkili verileri (navigation properties) tek bir veritabanı sorgusuyla getirmek (Eager Loading) için kullanılır. Include ana ilişkiyi yüklerken, ThenInclude bu yüklenen ilişki üzerindeki alt ilişkilere ulaşmayı sağlar. Performansı artırır ve N+1 sorgu sorununu önler.
        /*
         Include (Birincil İlişki): Ana entity'ye bağlı olan doğrudan ilişkili veriyi getirir.
         Örnek: Bir Post tablosu ile Comment tablosu varsa, Postları getirirken Commentleri de beraberinde getirmek.
         ThenInclude (Alt/Zincirleme İlişki): Include ile getirilen veriye bağlı olan başka bir tabloyu getirir.
         Örnek: Post -> Comment -> Author (Yorumun yazarı) ilişkisi varsa, Postları getir, Commentleri Include et, sonra ThenInclude ile Commentlerin yazarlarını (Author) getir.
        // Bu Include işlemini List olsun veya sadece o entity'nin type'ında Field olsun farketmeksizin başka bir entity'den veri çekmek için, bu işlemi yapmak lazım. 
        */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ogrenci = await _Context
                        .Ogrenciler
                        .Include(o => o.KursKayitlari)
                        .ThenInclude(o => o.Kurs)
                        .FirstOrDefaultAsync(o => o.OgrenciId == id);
            if (ogrenci == null)
            {
                return NotFound();
            }
            return View(ogrenci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Bu attribute bir güvenlik attribute'ü. Şu işe yarıyor: Formu açtığımızda asp.net core bizim için formun içerisine otomatik bir hidden input ekliyor. Bu input'un name:_RequestVerifivationToken value'sunda da globade spesifik bir id veriyor. Bu id'yi vermesinin sebebi formu get edenle post eden aynı kişimi buna bakıyor. Yani başkası bize linke tıklatıp formu post edebilir. Bunu önüne geçmek için bu kontrolü tetikleyen Attribute ValidateAntiForgeryToken. 
        public async Task<IActionResult> Edit(int? id, Ogrenci model)
        {
            if (id != model.OgrenciId) // Burada model'den OgrenciId ile route'dan gelen id'yi karşılaştırdık.
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
                catch (DbUpdateConcurrencyException) // Veri tabanı güncelleme eşzamanlılık hatası 
                { // Burada yaptığımız: eğer veri tabanı güncellemede bir hata çıkarsa, bu if bloğuna gir. Any(herhangi) metodu ile model'den gelen data'ını database'de olup olmadığını kontrol ediyoruz. Eğer yoksa Notfound() yardımcı metodu dönüyor. Eğer varsa da Exception hatası fırlatılıyor.
                    if (!_Context.Ogrenciler.Any(o => o.OgrenciId == model.OgrenciId))
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
        public async Task<IActionResult> Delete(int? id) // Buradaki id parametresi route'tan yani url'den geliyor.
        {
            if (id == null)
            {
                return NotFound();
            }
            var ogrenci = await _Context.Ogrenciler.FindAsync(id);
            if (ogrenci == null) 
            {
                return NotFound();
            }
            return View(ogrenci);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm]int id/*,int ogrenciId*/) 
        {
            var ogrenci = await _Context.Ogrenciler.FindAsync(id);
            if (ogrenci == null)
            {
                return NotFound();
            }
            _Context.Remove(ogrenci);
            var kursKayit = await _Context.KursKayitlari.Where(o => o.OgrenciId == id).ToListAsync();
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
        // Yukarıdaki Delete(post) action'unda, id bilgiside route'tan geliyor. Ancak biz formdan gelen OgrenciId bilgisini almak istiyoruz. Çünkü form'a asp-for ile böyle bir bilgi girdisi yaptık. Model binding'de bir çok yerden parametre gelebilir. Route, form, query'den alabilir. Bu yüzden bu parametreyi doğru yönledirmemiz gerekli. Yani veriyi doğru yerden yakalaması gerekli. Bunun bir kaç yol var.
        // Ya buradaki parametrenin ismini, formda ki id bilgisinin name'i ile aynı yaparız. Yani burayıda OgrenciId yaparız. Çünkü route pattern'ında 3. değer id ismi ile tanımlanmıştır.
        // Veya bir parametre daha alırız. Yani ilk parametre route'dan gelen id olur. İkinci parametre ise formdan gelen OgrenciId olur.
        // Veya formdan gelen OgrenciId'nin name'ini buradaki gibi id yaparız. Sonra bu parametrenin başına [Fromform] yazarız. Yani formdan gelen id olduğunu söyleriz. Bu seneryoda diyelimki route'dan gelen id'yi istiyoruz. O zamanda yine parametrenin başına [FromRoute] yazarız. Bu attribute'ler microsoft leard'de Model Binding konusunda mevcut.    
    }
}
