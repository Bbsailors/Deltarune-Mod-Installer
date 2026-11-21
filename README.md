# DELTARUNE Mod Installer

Normalde DELTARUNE'a aynı anda **60 FPS** modunu, konsol sürümündeki **Border (kenarlık)** modunu ve hazırladığım **Yenilenmiş Türkçe Menü** modunu kurmaya çalıştığınızda aynı kod dosyalarını (`obj_time`, `obj_darkcontroller`, `DEVICE_MENU` vb.) değiştirdikleri için birbirlerini bozuyorlardı.

Bu araç, **UTMT CLI** kullanarak seçtiğiniz modları doğrudan oyununuzun `data.win` dosyalarına uygular. Birden fazla mod seçtiğinizde çakışan kodları otomatik olarak birleştirir; böylece istediğiniz kombinasyonu (tekli, ikili veya üçü bir arada) sorunsuz kurabilirsiniz.

---

### Paketteki Modlar

* **Yenilenmiş Türkçe Menü Modu:** Başlatıcıdaki (Launcher) bölüm seçme ekranını ve Bölüm 1-5 kayıt ekranlarını yatay kart tasarımına çevirir. Her bölümün kendi temasına uygun animasyonlu arka planı vardır ve oyunun fontlarına Türkçe karakterler (`ğ, ü, ş, ı, ö, ç`) eklenmiştir. 30 FPS veya 60 FPS fark etmeksizin çalışır.
* **60 FPS Modu:** Oyun hızını bozmadan tüm bölümleri (Başlatıcı + Bölüm 1-5) 60 FPS akıcılığına çıkarır ve oyun içi CONFIG menüsüne *Mod Settings* ekler.
* **Konsol Border Modu (NXRUNE):** Oyunu 16:9 geniş ekran pencere yapısına alır ve konsol sürümündeki dinamik kenarlıkları açar. 60 FPS moduyla beraber kurulduğunda CONFIG menüsünde hem *Border* hem *Mod Settings* ayarı bir arada çalışır.

---

### İndirme ve Kurulum

Hazır kurulum paketlerini sağ taraftaki **[Releases](../../releases)** kısmından işletim sisteminize göre indirebilirsiniz:

* **Windows için:** `DELTARUNE-Mod-Installer-Windows.zip` dosyasını indirip klasöre çıkartın ve `DeltaruneModInstaller.exe`'yi çalıştırın.
* **Linux / Steam Deck için:** `DELTARUNE-Mod-Installer-Linux.zip` dosyasını indirip klasöre çıkartın ve terminalden `./install-linux.sh` komutunu çalıştırın.

Oyun klasörünüz otomatik olarak algılanır (bulamazsa *Klasör Seç* butonundan DELTARUNE klasörünü gösterebilirsiniz). İstediğiniz modları işaretleyip **Seçili Modları Kur** demeniz yeterli.

> **Yedekleme Notu:** Kurulum başlamadan önce orijinal `data.win` dosyalarınız oyun klasörünüzdeki `mod_backups` içine otomatik yedeklenir. İstediğiniz zaman mod seçiminizi değiştirip tekrar kurabilir veya **Orijinale Dön** butonuyla oyunu ilk haline döndürebilirsiniz.

---

### English

If you try installing the **60 FPS Mod** and the **Console Border Mod (NXRUNE)** together—or combining them with the **Redesigned Card Menu**—they normally overwrite each other's code entries (`obj_time`, `obj_darkcontroller`, `DEVICE_MENU`).

This installer uses **UTMT CLI** to patch your `data.win` files and automatically merges overlapping GML code based on the mods you select.

1. Download the `Windows` or `Linux` zip from **[Releases](../../releases)** and extract it.
2. Run `DeltaruneModInstaller.exe` on Windows, or `./install-linux.sh` on Linux / Steam Deck.
3. You can switch the installer UI to English using the **EN** button at the top right.
4. Select the mods and chapters you want, then click **Install Selected Mods**.

Your clean vanilla `data.win` files are automatically backed up to `mod_backups/` before patching, so you can change your mod combination anytime or click **Restore Original** to revert back to vanilla.

---

### Kaynak Koddan Derleme (Building from Source)

Windows arayüzü ek bir bağımlılık gerektirmeden .NET Framework `csc.exe` ile doğrudan derlenebilir:

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /win32icon:src\app_icon.ico /out:DeltaruneModInstaller.exe src\InstallerApp.cs
```

### Emeği Geçenler / Credits

* **Yenilenmiş Menü Modu, Türkçe Font & Birleştirici Installer:** Proje Sahibi
* **NXRUNE (Console Borders):** Orijinal NXRUNE scripti yapımcıları
* **60 FPS Modu:** Orijinal DELTARUNE 60 FPS scripti yapımcıları
* **UTMT Altyapısı:** UndertaleModTool (`UndertaleModLib` / `Underanalyzer`) & `utmt-agent` CLI
