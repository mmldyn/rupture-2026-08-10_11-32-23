# 🚀 Project Progress Tracker

Welcome to the development progress repository! Below is the detailed breakdown of our milestones, features, and system architecture updates across our development timeline.

---

## 📅 Timeline & Milestone Summary

| Milestone Date | Features / Modules | Description / Sub-features | Status |
| :--- | :--- | :--- | :--- |
| **August 11** | **Game Physics** | Fisika Gempa dan Bangunan | 🟢 Selesai |
| | **Logika Massa** | Rigidbody Fisikal Bangunan | 🟢 Selesai |
| | **Random Sampling** | Pemilihan Durasi dan SR Gempa | 🟢 Selesai |
| | **Konversi SR ke UM** | Formula: $UM = (SR - 3.0) \times 0.075$ | 🟢 Selesai |
| **August 16** | **Player Health** | Berdasarkan Velocity dan Massa | 🟢 Selesai |
| | **Mekanika DCHO** | Drop, Cover, Hold On dan Safe Zone | 🟢 Selesai |
| | **Interaksi Universal** | Raycast System | 🔵 Dalam Proses |
| | **Time Manager** | Sistem Pemilihan Cuaca, dan Waktu | 🟢 Selesai |
| | **Cloud Generator** | Awan 3D Volumetrik | 🟢 Selesai |
| | **Audio Manager** | Gempa Berlangsung | 🟢 Selesai |
| **August 24** | **Kompas Navigasi** | Manipulasi Y Point dan UV Rect | ⏳ Mendatang |
| | **MiniMap** | Interaktif Minimap | ⏳ Mendatang |
| | **Guide Evakuasi** | Lines ke Titik Evakuasi | ⏳ Mendatang |
| | **Partikel Debu** | Debu dengan Deteksi Physics | ⏳ Mendatang |

---

## 🛠️ Detail Modul & Fitur

### 📌 Milestone: August 11
1. **Game Physics**: Implementasi dasar fisika gempa dan dampaknya terhadap struktur bangunan.
2. **Logika Massa**: Pengaturan properti *Rigidbody* fisik untuk simulasi berat dan beban bangunan.
3. **Random Sampling**: Algoritma pemilihan acak untuk durasi dan skala richter (SR) gempa.
4. **Konversi SR ke UM**: Penerapan formula matematis konversi Skala Richter (SR) ke Intensitas Mercalli (UM):
   $$\text{UM} = (SR - 3.0) \times 0.075$$

### 📌 Milestone: August 16
1. **Player Health**: Sistem kesehatan karakter yang dinamis berdasarkan kecepatan (*velocity*) dan massa (*mass*).
2. **Mekanika DCHO**: Simulasi protokol keselamatan bencana **Drop, Cover, Hold On** serta deteksi zona aman (*Safe Zone*).
3. **Interaksi Universal** *(Current Focus)*: Pengembangan sistem interaksi berbasis *Raycast System* yang fleksibel untuk berbagai objek.
4. **Time Manager**: Pengelolaan sistem lingkungan termasuk pemilihan cuaca dan siklus waktu.
5. **Cloud Generator**: Pembuatan sistem awan 3D volumetrik untuk visualisasi atmosfer.
6. **Audio Manager**: Manajemen efek suara dinamis saat gempa berlangsung.

### 📌 Milestone: August 24
1. **Kompas Navigasi**: Navigasi arah menggunakan manipulasi titik Y dan *UV Rect*.
2. **MiniMap**: Peta mini interaktif untuk orientasi pemain di dalam scene.
3. **Guide Evakuasi**: Panduan jalur garis (*lines*) penunjuk arah menuju titik evakuasi terdekat.
4. **Partikel Debu**: Efek visual partikel debu yang terintegrasi dengan deteksi fisik lingkungan.

---

## 📈 Status Legend
- 🟢 **Selesai** (*Completed*)
- 🔵 **Dalam Proses** (*In Progress*)
- ⏳ **Mendatang** (*Upcoming*)
