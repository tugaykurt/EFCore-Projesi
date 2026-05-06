namespace EFCoreApp
{
    public class EFCoreKurulumu
    {
        // ************************ EntityFreamework Core ***********************************
        // Entity framework core kurulumu için ilk olarak belirli paketleri kurmamız gerekli:
        // Microsoft.EntityFrameworkCore
        // Microsoft.EntityFrameworkCore.Tools
        // Microsoft.EntityFrameworkCore.SqlServer(Kullanılan Sql türü)
        // Microsoft.EntityFrameworkCore.Design
        // Burada önemli olan, paketleri kurarken versiyonlarının .Net core versiyonu ile aynı olması. Örn: .Net 9.0 kullanıyorsak, EFCore 9.0.14 olabilir. Ama versiyon sayısın ilk sayısı aynı olmalı. Ayrıca bu 4 paketin versiyonları da tamamıyla aynı olmalı. EFCore 9.0.14 kurduysak hepsi 9.0.14 versiyonunda olmalı.
        // Veya Package Manager Console ile de microsoft'un sayfasından bağlantı bilgilerini alarak bu paketleri kurabiliriz.
        // Bu projede sqlite database'ini kullanacağız. Sqlite mssql, mysql, oracle gibi server tabanlı bir database uygulaması değil. Dosya tabanlı bir uygulama. Verileri bir excel dosyası gibi kaydediyor. O yüzden kullanımı çok rahat.

        // Entity class'larını oluşturalım. Entity class'ları, database'deki table'lara karşılık gelen class'larıdır. Eğer biz database'den bir insanın verisini çekersek, burada insan table'ına karşılık gelen entity class'ının objesi üzerinden veriyi çekmiş olacağız. Data klasöründe ki Ogrenci.cs dosyasında git ve diğer entity'lerede bak... 

        // DbContext class'ını oluşturalım. DbContext class'ını oluşturmak için, isminin sonunda Context geçen bir class'ı DbContext class'ından türetmemiz lazım. Data klasöründe ki, DataContext.cs dosyasına git...

        // Bu aşamada, oluşturduğumuz DbContext class'ından uygulamanın haberi yok. Uygulamada bu DataContext class'ının nesnesini tanıtmamız gerekli, yani bildirmemiz gerekli. Biz instance türetmek için nereye kayıt yapıyorduk? Servise Collection'a. Bunuda Program.cs'deki, IoC container'ın, Servise Collection alanına service registration(servis kaydı) oluştumamız gerekli. Program.cs dosyasına git...
        /*builder.Services.AddDbContext<DataContext>(options =>
          {
            var config = builder.Configuration; // Burada; WepApplicationBuilder class'ının ConfigrationManager type'ında olan Configration property'sini çağırıyoruz. Bu bize ConfigrationManager type'ında bir instance veriyor. Bu instance, appsettings.json dosyasına erişebilmemizi sağlıyor. 
            var connectionStrings = config.GetConnectionString("Database"); // Daha sonra da burada config instance'ı üzerinden GetConnectionString() metodunu çağırıyoruz. Ve bu metoda oluşturduğumuz bağlantının ismini veriyoruz. Bu bize eriştiğimiz appsettings.json dosyasından, parametre olarak verdiğimiz, database bağlantı yolunun value'sini, yani içeriğini veriyor.  
            options.UseSqlite(connectionStrings); // Burada da aldığımız bağlantıyı Sqlite'ın provider'ı üzerine gönderip, bu projenin buluduğu dosyaya bir server oluşturmuş oluyoruz.
           });*/

        // DbContext class'ının servis registration işlemini yaptıktan sonra, database bağlantı yolu eklememiz gerekli. Yani connectionStrings(bağlantı cümlesi) eklememiz gerekli. Bunuda appsettings.json dosyasında yapmamız gerekli. Ancak projemiz gelişim aşamasında olduğu için biz appsettings.json dosyasındın altındaki appsettings.Development.json dosyasında bu işlemi yapacağız. Proje canlıya alındığıda appsettings.json dosyasında tanımlanmalıdır. appsettings.Development.json dosyasına git(Tabi burada ki bağlantı Sqlite için geçerli;
        //"ConnectionStrings": {"Database": "Data Source=mydb.db"}
        // Diğer sql server'ları farklı connectionStrings pattern'ları içeriyor)...

        // Not: Sqlite kullanımında database oluşturulduktan sonra projenin sağdaki dosya dizinin içinde bir sqlite dosyası olacak. Bu dosyayı açabilmek için Sqlite browser'a ihtiyacımız var. Bu dosyayı sqlite browser'a sürükleyip bıraktığımızda bütün database'i görüntülemiş oluruz.  

        // EntityFramework Core database bağlantısı:
        // Programın ayağa kaltığı katmandan, yani UI katmanında, appsettings.json dosyasına gidiyoruz. Dosya içerisinde en üste veya en alta, en dış süslü parantezlerin içerisine, "ConnectionStrings":{"DefaultConnection": "Server=TUGAY-KURT;Database=Database'in ismi; User Id=sa; Password=160776; TrustServerCertificate=True"} bu şekilde EF core'da database bağlantısını yapmış oluyoruz. Daha sonra service collection'a kayıt yapmamız gerekli. Service collection'a gidip:
        // Burasına mssql sqlserver ile bağlantı registration'u.
        /*builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });*/ // Bu işlemi yapmamız gerekli.

        // Database bağlantısını gerçekleştirdikten sonra, migration oluşturmamız gerekli.
        // ASP.NET Core Migration, Entity Framework Core (EF Core) Code-First yaklaşımı kullanılarak, C# kodunda yapılan model değişikliklerini (tablo ekleme, sütun değiştirme vb.) otomatik olarak SQL komutlarına dönüştürüp veritabanı şemasına uygulayan bir sürüm kontrol mekanizmasıdır. Veritabanı şemasını yönetmek, değişiklikleri takip etmek ve güvenli bir şekilde güncellemek için kullanılır.
        // Migration ekleme işlemlerinde EFCore ve EF arasındaki tek fark, EF'de bütün komutlardan önce Enable-Migrations komutunu çalıştırmak.
        // EF Core'da Migration oluşturmak için, belli bir kaç komuta ihtiyacımız var. İlk olarak Package Manager Console'a gelip, console'un üzerindeki Default project kısmını EFcore'u kurduğumuz katmanı seçeriz. Daha sonrada:
        // Add-Migration Migraionİsmi(Migration klasörü ve içine migration oluşturur.)
        // Update-Database(database oluşturur, table'ları oluşturur veya bunları günceller.)
        // Bu klasörün içerisinde oluşan migration class'ı haricinde oluşan dosya bir fluent configration pattern'dır. Yani aslın bir confiration'dur. 
        // Artık Migration sayesinde database'imiz oluşmuş olur.
        // Eğer biz Database'i güncellemek istersek. Şunu yapmamamız gerekli:
        // Yeni bir Migration adı belirleyerek, yukarıdaki migration ekleme işlemlerinin aynısını yapmamız gerekli. Yani;
        // Add-Migration YeniMigrationIsmi 
        // Update-Database
        // Dememiz gerekli.

        // Bu aşamada database'imizin tablolarında bir veri olmayacak, Ancak biz database'i test etmek istersek örnek veriler test verilerine ihtiyacımız olacak. Bunun içinde bir Seed Data oluşturmamız gerekli. Data klasöründe ki, SeedData.cs dosyasına git...  


        // ************** Table'lar/Model'ler Arasındaki İlişkiler ********************
        /*
          1. Bire Bir (One-to-One) İlişki
            Bir tablodaki bir kaydın, diğer tablodaki sadece bir kayıtla eşleşmesidir. Genellikle güvenlik veya performans nedeniyle verileri ayırmak için kullanılır.
            Örnek: Kullanıcı ve Profil.
            Bir kullanıcının sadece bir profil sayfası vardır.
            Bir profil sayfası sadece bir kullanıcıya aittir.
            Nasıl Kurulur? İki tablodan birine diğerinin ID'si eklenir (genelde unique yani benzersiz olarak).


          2. Bire Çok (Many To One) İlişki 
            En sık kullanılan modeldir. Bir ana kayıt, karşı tarafta birçok alt kayda sahip olabilir.
            Örnek: Müşteri ve Siparişler.
            Bir müşteri onlarca sipariş verebilir.
            Ancak o sipariş fişi (ID'si) sadece o müşteriye aittir.
            Nasıl Kurulur? "Çok" olan tarafa (Sipariş), "Bir" olan tarafın (Müşteri) ID'si eklenir.

          3. Çoka Çok (Many-to-Many) İlişki
            Bir tablodaki birden fazla kaydın, diğer tablodaki birden fazla kayıtla ilişkili olması durumudur.
            Örnek: Öğrenciler ve Dersler.
            Bir öğrenci birden fazla ders alabilir.
            Bir dersin de birden fazla öğrencisi olabilir.
            Nasıl Kurulur? Bu iki tablo doğrudan birbirine bağlanamaz. Araya "Ara Tablo" (Pivot Table) denilen üçüncü bir tablo eklenir. Bu tablo her iki tarafın ID'lerini eşleştirerek köprü görevi görür.

          4. Öz-Yinelemeli (Self-Referencing) İlişki
            Bir tablonun kendi kendisiyle ilişki kurmasıdır. Hiyerarşik yapılar için kullanılır.
            Örnek: Çalışanlar ve Yöneticiler.
            Bir çalışan, yine aynı tabloda bulunan başka bir çalışana (yöneticisine) bağlıdır.
            Yönetici de aslında o tabloda bir "çalışan" kaydıdır.
            Nasıl Kurulur? Tabloya ust_id veya manager_id gibi, yine kendi ID'sine işaret eden bir sütun eklenir.

          Özetle:
            1-1: Eşler (Tekil eşleşme).
            1-N: Ebeveyn-Çocuk (Hiyerarşik).
            N-N: Arkadaşlar (Karmaşık ağ, ara tablo şart).
            Self: Aile ağacı (Kendi içinde bağ).

            // Model'leri ilişkilendirirken, model'in içerisinde bir başka model'in Id'sinin property'sini açmamızın sebebi, başka model'in bu model'le ilişkilenen verisini bulmak için kullanırız.
            // Model'leri ilişkilerndiriken, model'in içerisinde bir başka model'in type'ında bir property açmamızın sebebi, başka model'in bu model'le ilişkilenen versini id'si üzerinden bulduktan sonra bu verileri alabilmek için kullanırız.
            // Model'leri ilişkilerndiriken, model'in içerisinde bir başka model'in type'ında bir List property açmamızın sebebi, başka model'in bu model'le ilişkilenen versini id'si üzerinden bulduktan sonra bu verileri liste halinde alabilmek için kullanırız.(bire çok veya çoka çok ilişkide kullanılır.)

            * Bire bir ilişkide: Örn:Kullanıcı ve Profil 
              Kullanıcı modeline: ProfilId ve Profil type'ında iki property açılır.
              Profil modeline: KullanıcıId ve Kullanıcı type'ında iki property açılır.

            * Bire çok ilişkide: Örn: Müşteri ve Siparişler
              Müşteri modelinde: SiparişId ve Sipariş type'ında List formunda, iki property açılır.
              Sipariş modelinde: MüşteriId ve Müşteri type'ında iki property açılır.

            * Çoka çok ilişkide: Örn: Öğrenciler ve Dersler
              Bu yapıda bir aracı model kullanırız. Bu aracı modelin içerisinde: 
              1.OgrenciId ve DersId property'leri açılır.
              2.Ogrenci ve Ders type'larında property'leri açılır.(Inner Join)
              Ayriyetten;
              3. Ogrenci model'inde: Bu aracı modelin list formunda property'sini açılır.(left join)
              4. Ders model'inde: Bu aracı modelin list formunda property'sini açılır.(right join)

            * Öz-Yinelemeli ilişkide: Örn: Çalışanlar ve Yöneticiler
            * Burada Calısanlar tek tablo/model vardır. Bu tablonun içerisinde ayrıcı bir id gereklidir. Örneğin sırdan çalışanların belli tip spesifik id'si, yönetici çalışanların ayrı tipte spesifik id'si olmalı. 
            * Ekstradan bu model'in içerisinde, sıradan çalışanların bağlı olduğu yöneticiyi belirlemek için ekstra bir BaglıOlduguYoneticiId şeklinde bir property daha eklememiz gerekli(bunun içerisinde bire çok veya çoka.ok ilişkide olabilir).

              Bu ilişkilendirmeleri yaptıktan sonra listelerken 2 tane metod kullanırız:

              ASP.NET Core (Entity Framework Core) içerisinde Include ve ThenInclude, ilişkili verileri (navigation properties) tek bir veritabanı sorgusuyla getirmek (Eager Loading) için kullanılır. Include ana ilişkiyi yüklerken, ThenInclude bu yüklenen ilişki üzerindeki alt ilişkilere ulaşmayı sağlar. Performansı artırır ve N+1 sorgu sorununu önler.

             Include (Birincil İlişki): Ana entity'ye bağlı olan doğrudan ilişkili veriyi getirir.
             Örnek: Bir Post tablosu ile Comment tablosu varsa, Postları getirirken Commentleri de beraberinde getirmek.
             ThenInclude (Alt/Zincirleme İlişki): Include ile getirilen veriye bağlı olan başka bir tabloyu getirir.
             Örnek: Post -> Comment -> Author (Yorumun yazarı) ilişkisi varsa, Postları getir, Commentleri Include et, sonra ThenInclude ile Commentlerin yazarlarını (Author) getir. 
             // Bu Include işlemini List olsun veya sadece o entity'nin type'ında Field olsun farketmeksizin başka bir entity'den veri çekmek için, bu işlemi yapmak lazım.  OgrenciController Edit action'una git... 

         // KursKayitController'a git... 
        */


    }
}
