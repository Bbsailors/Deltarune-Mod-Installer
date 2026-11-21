#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
DELTARUNE Mod Installer (Linux & Cross-Platform Edition)
Modern, minimalist dark-themed GUI (Tkinter) + Settings Menu + TR/EN Localization + CLI fallback.
"""

import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
import threading
import uuid
from datetime import datetime
from pathlib import Path

ROLE_INFO = [
    ("launcher", "Başlatıcı", "Launcher", "data.win", "apply_60fps_launcher.csx"),
    ("ch1", "Bölüm 1", "Chapter 1", "chapter1_windows/data.win", "apply_60fps_ch1.csx"),
    ("ch2", "Bölüm 2", "Chapter 2", "chapter2_windows/data.win", "apply_60fps_ch2.csx"),
    ("ch3", "Bölüm 3", "Chapter 3", "chapter3_windows/data.win", "apply_60fps_ch3.csx"),
    ("ch4", "Bölüm 4", "Chapter 4", "chapter4_windows/data.win", "apply_60fps_ch4.csx"),
    ("ch5", "Bölüm 5", "Chapter 5", "chapter5_windows/data.win", "apply_60fps_ch5.csx"),
]

MODS_INFO = [
    (
        "menu",
        ("Yenilenmiş Türkçe Menü", "Redesigned Menu & Turkish Mod"),
        ("Arayüz & Türkçe", "UI & Turkish Font"),
        (
            "Yatay kart tasarımlı bölüm ve kayıt seçimi, bölüme özel animasyonlu arka planlar ve Türkçe font.",
            "Horizontal card UI for chapter & save selection, animated chapter backgrounds, and Turkish pixel font.",
        ),
    ),
    (
        "fps60",
        ("60 FPS Modu", "60 FPS Mod"),
        ("Performans", "Performance"),
        (
            "Oyun hızını değiştirmeden tüm hareket, savaş, ara sahne ve animasyonları 60 FPS akıcılığına yükseltir.",
            "Upgrades all movement, battles, cutscenes, and UI animations to smooth 60 FPS while preserving game speed.",
        ),
    ),
    (
        "border",
        ("Konsol Border Modu", "Console Border Mod (NXRUNE)"),
        ("16:9 Çerçeve", "16:9 Widescreen"),
        (
            "Geniş ekran pencere görünümü, konsol sürümündeki dinamik kenarlıklar ve ayarlar menüsü seçeneği.",
            "16:9 widescreen window, dynamic console borders per chapter, and Border option in the CONFIG menu.",
        ),
    ),
]

ACCENT_THEMES = [
    ("İndigo", "Indigo", "#6366F1", "#4F46E5", "#818CF8"),
    ("Siber Mavi", "Cyber Cyan", "#0EA5E9", "#0284C7", "#38BDF8"),
    ("Kızıl Ruh", "Soul Crimson", "#F43F5E", "#E11D48", "#FB7185"),
    ("Altın Sarısı", "Royal Gold", "#EAB308", "#CA8A04", "#FACC15"),
    ("Zümrüt Yeşili", "Emerald Green", "#10B981", "#059669", "#34D399"),
]


def get_base_dir() -> Path:
    return Path(__file__).resolve().parent


def load_settings() -> dict:
    cfg = {"lang": "tr", "accent": 0, "autolaunch": False, "verbose": False, "confirm": False}
    p = get_base_dir() / "installer_settings.ini"
    if p.is_file():
        try:
            for line in p.read_text(encoding="utf-8").splitlines():
                if "=" not in line:
                    continue
                k, v = [x.strip() for x in line.split("=", 1)]
                ku = k.upper()
                if ku == "LANG":
                    cfg["lang"] = "en" if v.lower() == "en" else "tr"
                elif ku == "ACCENT":
                    cfg["accent"] = max(0, min(4, int(v)))
                elif ku == "AUTOLAUNCH":
                    cfg["autolaunch"] = v in ("1", "true", "True")
                elif ku == "VERBOSE":
                    cfg["verbose"] = v in ("1", "true", "True")
                elif ku == "CONFIRM":
                    cfg["confirm"] = v in ("1", "true", "True")
        except Exception:
            pass
    return cfg


def save_settings(cfg: dict):
    p = get_base_dir() / "installer_settings.ini"
    try:
        p.write_text(
            f"LANG={cfg['lang']}\n"
            f"ACCENT={cfg['accent']}\n"
            f"AUTOLAUNCH={'1' if cfg['autolaunch'] else '0'}\n"
            f"VERBOSE={'1' if cfg['verbose'] else '0'}\n"
            f"CONFIRM={'1' if cfg['confirm'] else '0'}\n",
            encoding="utf-8",
        )
    except Exception:
        pass


def is_valid_deltarune_dir(p: Path) -> bool:
    if not p or not p.is_dir():
        return False
    return (p / "data.win").is_file() or (p / "chapter1_windows" / "data.win").is_file()


def auto_detect_game_path() -> str:
    base = get_base_dir()
    home = Path.home()
    candidates = [
        base,
        base.parent,
        base.parent / "DELTARUNE",
        home / ".local/share/Steam/steamapps/common/DELTARUNE",
        home / ".steam/steam/steamapps/common/DELTARUNE",
        home / ".steam/root/steamapps/common/DELTARUNE",
        home / ".var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/DELTARUNE",
        home / "snap/steam/common/.local/share/Steam/steamapps/common/DELTARUNE",
        Path("/run/media/mmcblk0p1/steamapps/common/DELTARUNE"),
        home / "Games/DELTARUNE",
    ]
    for c in candidates:
        try:
            if is_valid_deltarune_dir(c):
                return str(c.resolve())
        except Exception:
            pass
    return ""


def detect_utmt_runner(base_dir: Path):
    utmt_dir = base_dir / "utmt-cli"
    utmt_exe = utmt_dir / "utmt-agent.exe"
    utmt_dll = utmt_dir / "utmt-agent.dll"
    linux_rt = utmt_dir / "utmt-agent.linux.runtimeconfig.json"

    if os.name == "nt" and utmt_exe.is_file():
        return ("native_exe", [str(utmt_exe)], "Windows UTMT CLI")

    wine_bin = shutil.which("wine64") or shutil.which("wine")
    if wine_bin and utmt_exe.is_file():
        return ("wine", [wine_bin, str(utmt_exe)], f"Wine ({Path(wine_bin).name})")

    home = Path.home()
    for pr in [
        home / ".local/share/Steam/steamapps/common",
        home / ".steam/steam/steamapps/common",
        home / ".var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common",
    ]:
        if pr.is_dir():
            for p_wine in sorted(pr.glob("Proton*/files/bin/wine64"), reverse=True):
                if p_wine.is_file() and utmt_exe.is_file():
                    return ("wine", [str(p_wine), str(utmt_exe)], f"Proton ({p_wine.parent.parent.parent.name})")

    dotnet_bin = shutil.which("dotnet")
    if dotnet_bin and utmt_dll.is_file():
        cmd = [dotnet_bin, "exec"]
        if linux_rt.is_file():
            cmd += ["--runtimeconfig", str(linux_rt)]
        cmd.append(str(utmt_dll))
        return ("dotnet", cmd, "Native Linux .NET")

    utmt_native = shutil.which("UndertaleModCli") or (
        str(utmt_dir / "UndertaleModCli") if (utmt_dir / "UndertaleModCli").is_file() else None
    )
    if utmt_native:
        return ("utmt_cli", [utmt_native], "Native UndertaleModCli")

    return (None, [], "Not Found (Install 'wine' or 'dotnet')")


def to_runner_path(runner_type: str, p: Path) -> str:
    resolved = str(p.resolve())
    if runner_type == "wine" and os.name != "nt":
        return "Z:" + resolved.replace("/", "\\")
    return resolved


def run_utmt_job(
    base_dir: Path,
    runner_type: str,
    cmd_prefix: list,
    data_path: Path,
    out_path: Path,
    script_path: Path,
    work_dir: Path,
    role: str,
    fps60: bool,
    border: bool,
    menu: bool,
    verbose: bool,
    log_fn,
) -> bool:
    mods_dir = base_dir / "mods"
    cfg_path = mods_dir / "patch_config.txt"
    cfg_path.write_text(
        f"ROLE={role}\n"
        f"FPS60={'1' if fps60 else '0'}\n"
        f"BORDER={'1' if border else '0'}\n"
        f"MENU={'1' if menu else '0'}\n",
        encoding="utf-8",
    )

    env = os.environ.copy()
    env["DR_ROLE"] = role
    env["DR_MOD_FPS60"] = "1" if fps60 else "0"
    env["DR_MOD_BORDER"] = "1" if border else "0"
    env["DR_MOD_MENU"] = "1" if menu else "0"
    if runner_type == "wine":
        env["WINEDEBUG"] = "-all"

    if runner_type == "utmt_cli":
        cmd = cmd_prefix + ["load", str(data_path), "-s", str(script_path), "-o", str(out_path)]
    else:
        job_file = work_dir / f"job_{role}_{uuid.uuid4().hex[:8]}.json"
        job_data = {
            "data_path": to_runner_path(runner_type, data_path),
            "out_path": to_runner_path(runner_type, out_path),
            "script_path": to_runner_path(runner_type, script_path),
            "exports_dir": to_runner_path(runner_type, work_dir),
            "compile_only": False,
        }
        job_file.write_text(json.dumps(job_data, indent=2), encoding="utf-8")
        cmd = cmd_prefix + ["script-worker", to_runner_path(runner_type, job_file)]

    succeeded = False
    proc = subprocess.Popen(
        cmd,
        cwd=str(base_dir / "utmt-cli"),
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        encoding="utf-8",
        errors="replace",
        env=env,
    )

    for raw_line in proc.stdout:
        line = raw_line.strip()
        if not line:
            continue
        if '"kind":"message"' in line:
            m = re.search(r'"text":"([^"]*)"', line)
            if m:
                log_fn("  " + m.group(1))
        elif verbose and '"kind":"progress"' in line:
            log_fn("  [UTMT] " + line)
        elif '"kind":"error"' in line:
            log_fn("  [UTMT ERROR] " + line)
        elif '"kind":"result"' in line and '"ok":true' in line:
            succeeded = True

    proc.wait()
    if proc.returncode == 0 and out_path.is_file():
        succeeded = True
    return succeeded


def run_gui():
    import tkinter as tk
    from tkinter import filedialog, messagebox

    cfg = load_settings()

    BG_MAIN = "#0B0D13"
    BG_HEADER = "#0F121A"
    BG_CARD = "#131722"
    BG_CARD_SEL = "#171C2C"
    BG_INPUT = "#10131C"
    BORDER_SUBTLE = "#232A3B"
    SUCCESS = "#34D399"
    DANGER = "#F87171"
    TEXT_PRIMARY = "#F1F5F9"
    TEXT_SECONDARY = "#94A3B8"
    TEXT_MUTED = "#64748B"

    def get_accent():
        _, _, acc, hov, lgt = ACCENT_THEMES[cfg["accent"]]
        return acc, hov, lgt

    root = tk.Tk()
    root.title("DELTARUNE Mod Installer")
    root.geometry("860x700")
    root.minsize(740, 620)
    root.configure(bg=BG_MAIN)

    icon_png = get_base_dir() / "utmt-cli" / "app_icon.png"
    if icon_png.is_file():
        try:
            icon_img = tk.PhotoImage(file=str(icon_png))
            root.iconphoto(True, icon_img)
        except Exception:
            pass

    is_fullscreen = [False]
    is_working = [False]

    def is_en():
        return cfg["lang"] == "en"

    def toggle_fullscreen(_=None):
        is_fullscreen[0] = not is_fullscreen[0]
        root.attributes("-fullscreen", is_fullscreen[0])
        apply_localization()

    root.bind("<F11>", toggle_fullscreen)
    root.bind("<Escape>", lambda e: toggle_fullscreen() if is_fullscreen[0] else None)

    # Header
    header = tk.Frame(root, bg=BG_HEADER, height=64)
    header.pack(fill="x", side="top")
    header.pack_propagate(False)

    header_inner = tk.Frame(header, bg=BG_HEADER)
    header_inner.pack(fill="both", expand=True, padx=28)

    lbl_title = tk.Label(header_inner, text="DELTARUNE", font=("Sans", 14, "bold"), fg=TEXT_PRIMARY, bg=BG_HEADER)
    lbl_title.pack(side="left", pady=16)

    _, _, runner_desc = detect_utmt_runner(get_base_dir())
    lbl_sub = tk.Label(header_inner, text="", font=("Sans", 9), fg=TEXT_SECONDARY, bg=BG_HEADER)
    lbl_sub.pack(side="left", padx=12, pady=18)

    btn_fs = tk.Button(
        header_inner,
        text="Tam Ekran (F11)",
        font=("Sans", 9),
        fg=TEXT_PRIMARY,
        bg=BG_CARD,
        activebackground=BG_CARD_SEL,
        activeforeground=TEXT_PRIMARY,
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
        padx=12,
        pady=6,
        cursor="hand2",
        command=toggle_fullscreen,
    )
    btn_fs.pack(side="right", pady=14)

    def open_settings_dialog():
        dlg = tk.Toplevel(root)
        dlg.title("Settings" if is_en() else "Ayarlar")
        dlg.geometry("430x380")
        dlg.resizable(False, False)
        dlg.configure(bg=BG_MAIN)
        dlg.transient(root)
        dlg.grab_set()

        wrap = tk.Frame(dlg, bg=BG_MAIN, padx=22, pady=18)
        wrap.pack(fill="both", expand=True)

        tk.Label(
            wrap,
            text="Application Settings" if is_en() else "Uygulama Ayarları",
            font=("Sans", 12, "bold"),
            fg=TEXT_PRIMARY,
            bg=BG_MAIN,
        ).pack(anchor="w", pady=(0, 14))

        # Language
        r_lang = tk.Frame(wrap, bg=BG_MAIN)
        r_lang.pack(fill="x", pady=4)
        tk.Label(r_lang, text="Language / Dil:", fg=TEXT_SECONDARY, bg=BG_MAIN, font=("Sans", 9)).pack(side="left")
        lang_var = tk.StringVar(value="English (EN)" if is_en() else "Türkçe (TR)")
        lang_menu = tk.OptionMenu(r_lang, lang_var, "Türkçe (TR)", "English (EN)")
        lang_menu.config(bg=BG_CARD, fg=TEXT_PRIMARY, bd=0, highlightthickness=1, highlightbackground=BORDER_SUBTLE)
        lang_menu.pack(side="right")

        # Accent
        r_acc = tk.Frame(wrap, bg=BG_MAIN)
        r_acc.pack(fill="x", pady=6)
        tk.Label(
            r_acc,
            text="Theme Accent:" if is_en() else "Tema Rengi:",
            fg=TEXT_SECONDARY,
            bg=BG_MAIN,
            font=("Sans", 9),
        ).pack(side="left")
        acc_names = [x[1] if is_en() else x[0] for x in ACCENT_THEMES]
        acc_var = tk.StringVar(value=acc_names[cfg["accent"]])
        acc_menu = tk.OptionMenu(r_acc, acc_var, *acc_names)
        acc_menu.config(bg=BG_CARD, fg=TEXT_PRIMARY, bd=0, highlightthickness=1, highlightbackground=BORDER_SUBTLE)
        acc_menu.pack(side="right")

        v_auto = tk.BooleanVar(value=cfg["autolaunch"])
        v_verb = tk.BooleanVar(value=cfg["verbose"])
        v_conf = tk.BooleanVar(value=cfg["confirm"])

        for var, tr_t, en_t in [
            (v_auto, "Kurulum sonrası oyunu otomatik başlat", "Auto-launch game after installation"),
            (v_verb, "Konsolda ayrıntılı UTMT CLI çıktılarını göster", "Show verbose UTMT CLI output in console"),
            (v_conf, "Kuruluma başlamadan önce onay iste", "Ask for confirmation before installing"),
        ]:
            tk.Checkbutton(
                wrap,
                text=en_t if is_en() else tr_t,
                variable=var,
                fg=TEXT_PRIMARY,
                bg=BG_MAIN,
                selectcolor=BG_INPUT,
                activebackground=BG_MAIN,
                activeforeground=TEXT_PRIMARY,
                font=("Sans", 9),
                bd=0,
                highlightthickness=0,
            ).pack(anchor="w", pady=3)

        acc_col, acc_hov, _ = get_accent()

        def save_and_close():
            cfg["lang"] = "en" if "EN" in lang_var.get() else "tr"
            sel_acc = acc_var.get()
            for idx, item in enumerate(ACCENT_THEMES):
                if sel_acc in (item[0], item[1]):
                    cfg["accent"] = idx
                    break
            cfg["autolaunch"] = v_auto.get()
            cfg["verbose"] = v_verb.get()
            cfg["confirm"] = v_conf.get()
            save_settings(cfg)
            apply_localization()
            dlg.destroy()

        tk.Button(
            wrap,
            text="Save & Apply" if is_en() else "Kaydet ve Uygula",
            font=("Sans", 10, "bold"),
            fg="#FFFFFF",
            bg=acc_col,
            activebackground=acc_hov,
            activeforeground="#FFFFFF",
            bd=0,
            pady=8,
            cursor="hand2",
            command=save_and_close,
        ).pack(fill="x", side="bottom", pady=(14, 0))

    btn_settings = tk.Button(
        header_inner,
        text="⚙ Ayarlar",
        font=("Sans", 9),
        fg=TEXT_PRIMARY,
        bg=BG_CARD,
        activebackground=BG_CARD_SEL,
        activeforeground=TEXT_PRIMARY,
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
        padx=12,
        pady=6,
        cursor="hand2",
        command=open_settings_dialog,
    )
    btn_settings.pack(side="right", padx=8, pady=14)

    def toggle_lang():
        cfg["lang"] = "tr" if is_en() else "en"
        save_settings(cfg)
        apply_localization()

    btn_lang = tk.Button(
        header_inner,
        text="EN",
        font=("Sans", 9, "bold"),
        fg=TEXT_PRIMARY,
        bg=BG_CARD,
        activebackground=BG_CARD_SEL,
        activeforeground=TEXT_PRIMARY,
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
        padx=10,
        pady=6,
        cursor="hand2",
        command=toggle_lang,
    )
    btn_lang.pack(side="right", pady=14)

    tk.Frame(root, bg=BORDER_SUBTLE, height=1).pack(fill="x", side="top")

    main_wrap = tk.Frame(root, bg=BG_MAIN)
    main_wrap.pack(fill="both", expand=True, padx=28, pady=16)

    # Path Card
    path_card = tk.Frame(main_wrap, bg=BG_CARD, highlightthickness=1, highlightbackground=BORDER_SUBTLE, padx=16, pady=10)
    path_card.pack(fill="x", pady=(0, 12))

    path_top = tk.Frame(path_card, bg=BG_CARD)
    path_top.pack(fill="x")

    lbl_path_cap = tk.Label(path_top, text="Oyun Klasörü", font=("Sans", 9, "bold"), fg=TEXT_SECONDARY, bg=BG_CARD)
    lbl_path_cap.pack(side="left")

    lbl_path_status = tk.Label(path_top, text="", font=("Sans", 9), fg=SUCCESS, bg=BG_CARD)
    lbl_path_status.pack(side="right")

    path_row = tk.Frame(path_card, bg=BG_CARD)
    path_row.pack(fill="x", pady=(6, 0))

    game_path_var = tk.StringVar(value=auto_detect_game_path())
    ent_path = tk.Entry(
        path_row,
        textvariable=game_path_var,
        font=("Sans", 10),
        fg=TEXT_PRIMARY,
        bg=BG_INPUT,
        insertbackground=TEXT_PRIMARY,
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
    )
    ent_path.pack(side="left", fill="x", expand=True, ipady=6, padx=(0, 10))

    def browse_folder():
        initial = game_path_var.get() if os.path.isdir(game_path_var.get()) else str(Path.home())
        d = filedialog.askdirectory(
            title="Select DELTARUNE folder" if is_en() else "DELTARUNE oyun klasörünü seçin",
            initialdir=initial,
        )
        if d:
            game_path_var.set(d)

    btn_browse = tk.Button(
        path_row,
        text="Klasör Seç",
        font=("Sans", 9),
        fg=TEXT_PRIMARY,
        bg=BG_CARD_SEL,
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
        padx=14,
        pady=5,
        cursor="hand2",
        command=browse_folder,
    )
    btn_browse.pack(side="right")

    # Mod Cards
    mod_vars = {}
    mod_widgets = {}

    for key, titles, tags, descs in MODS_INFO:
        var = tk.BooleanVar(value=True)
        mod_vars[key] = var
        acc_c, _, acc_l = get_accent()

        card = tk.Frame(main_wrap, bg=BG_CARD_SEL, highlightthickness=1, highlightbackground=acc_c, padx=16, pady=12, cursor="hand2")
        card.pack(fill="x", pady=(0, 10))

        row1 = tk.Frame(card, bg=BG_CARD_SEL)
        row1.pack(fill="x")

        chk_lbl = tk.Label(row1, text="✓", font=("Sans", 10, "bold"), fg="#FFFFFF", bg=acc_c, width=2, padx=2, pady=1)
        chk_lbl.pack(side="left", padx=(0, 12))

        title_lbl = tk.Label(row1, text=titles[0], font=("Sans", 10, "bold"), fg=TEXT_PRIMARY, bg=BG_CARD_SEL)
        title_lbl.pack(side="left")

        tag_lbl = tk.Label(row1, text=tags[0], font=("Sans", 8), fg=acc_l, bg="#1E2238", padx=8, pady=1)
        tag_lbl.pack(side="left", padx=10)

        desc_lbl = tk.Label(card, text=descs[0], font=("Sans", 9), fg=TEXT_SECONDARY, bg=BG_CARD_SEL, anchor="w", justify="left")
        desc_lbl.pack(fill="x", padx=(34, 0), pady=(4, 0))

        mod_widgets[key] = (card, row1, chk_lbl, title_lbl, tag_lbl, desc_lbl, titles, tags, descs)

        def make_toggler(k=key):
            def toggle(_=None):
                if is_working[0]:
                    return
                mod_vars[k].set(not mod_vars[k].get())
                refresh_cards_style()

            return toggle

        t_fn = make_toggler()
        for w in (card, row1, chk_lbl, title_lbl, tag_lbl, desc_lbl):
            w.bind("<Button-1>", t_fn)

    def refresh_cards_style():
        acc_c, _, acc_l = get_accent()
        idx = 1 if is_en() else 0
        for k, (card, row1, chk_lbl, title_lbl, tag_lbl, desc_lbl, titles, tags, descs) in mod_widgets.items():
            v = mod_vars[k].get()
            bg_c = BG_CARD_SEL if v else BG_CARD
            bd_c = acc_c if v else BORDER_SUBTLE
            card.config(bg=bg_c, highlightbackground=bd_c)
            row1.config(bg=bg_c)
            chk_lbl.config(text="✓" if v else " ", bg=acc_c if v else BG_INPUT, fg="#FFFFFF" if v else TEXT_MUTED)
            title_lbl.config(text=titles[idx], bg=bg_c, fg=TEXT_PRIMARY if v else TEXT_SECONDARY)
            tag_lbl.config(text=tags[idx], fg=acc_l if v else TEXT_MUTED)
            desc_lbl.config(text=descs[idx], bg=bg_c)

    # Chapters Bar
    ch_card = tk.Frame(main_wrap, bg=BG_CARD, highlightthickness=1, highlightbackground=BORDER_SUBTLE, padx=14, pady=8)
    ch_card.pack(fill="x", pady=(2, 12))

    ch_vars = []
    ch_checks = []
    for _, tr_name, _, _, _ in ROLE_INFO:
        cv = tk.BooleanVar(value=True)
        ch_vars.append(cv)
        cb = tk.Checkbutton(
            ch_card,
            text=tr_name,
            variable=cv,
            font=("Sans", 9),
            fg=TEXT_PRIMARY,
            bg=BG_CARD,
            selectcolor=BG_INPUT,
            activebackground=BG_CARD,
            activeforeground=TEXT_PRIMARY,
            bd=0,
            highlightthickness=0,
            cursor="hand2",
        )
        cb.pack(side="left", expand=True)
        ch_checks.append(cb)

    # Progress & Status
    lbl_progress = tk.Label(main_wrap, text="Kuruluma hazır", font=("Sans", 9), fg=TEXT_SECONDARY, bg=BG_MAIN, anchor="w")
    lbl_progress.pack(fill="x", pady=(0, 4))

    prog_bg = tk.Canvas(main_wrap, height=6, bg="#181D2A", highlightthickness=0)
    prog_bg.pack(fill="x", pady=(0, 12))
    prog_rect = prog_bg.create_rectangle(0, 0, 0, 6, fill=get_accent()[0], width=0)

    def set_progress(pct: int, text: str):
        def _u():
            lbl_progress.config(text=text)
            w = prog_bg.winfo_width()
            prog_bg.coords(prog_rect, 0, 0, int((w * max(0, min(100, pct))) / 100), 6)

        root.after(0, _u)

    btn_row = tk.Frame(main_wrap, bg=BG_MAIN)
    btn_row.pack(fill="x", side="bottom", pady=(12, 0))

    txt_log = tk.Text(
        main_wrap,
        height=6,
        bg=BG_INPUT,
        fg=TEXT_SECONDARY,
        font=("Monospace", 9),
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
        padx=10,
        pady=8,
        state="disabled",
    )
    txt_log.pack(fill="both", expand=True)

    def append_log(msg: str):
        def _a():
            ts = datetime.now().strftime("%H:%M:%S")
            txt_log.config(state="normal")
            txt_log.insert("end", f"[{ts}] {msg}\n")
            txt_log.see("end")
            txt_log.config(state="disabled")

        root.after(0, _a)

    def validate_path(*_):
        p = Path(game_path_var.get().strip())
        if not is_valid_deltarune_dir(p):
            lbl_path_status.config(
                text="● No valid DELTARUNE folder selected" if is_en() else "● Geçerli bir DELTARUNE klasörü seçilmedi",
                fg=DANGER,
            )
            return
        found = 0
        for i, (_, _, _, rel, _) in enumerate(ROLE_INFO):
            exists = (p / rel).is_file()
            if exists:
                found += 1
            ch_checks[i].config(state="normal" if (exists and not is_working[0]) else "disabled")
            if not exists:
                ch_vars[i].set(False)
        has_backup = (p / "mod_backups").is_dir()
        if is_en():
            lbl_path_status.config(text=f"● {found}/6 chapters ready" + ("  •  Backup available" if has_backup else ""), fg=SUCCESS)
        else:
            lbl_path_status.config(text=f"● {found}/6 bölüm hazır" + ("  •  Yedek mevcut" if has_backup else ""), fg=SUCCESS)

    game_path_var.trace_add("write", validate_path)

    def set_working(working: bool):
        def _w():
            is_working[0] = working
            state = "disabled" if working else "normal"
            ent_path.config(state=state)
            btn_browse.config(state=state)
            btn_install.config(state=state)
            btn_restore.config(state=state)
            btn_settings.config(state=state)
            validate_path()

        root.after(0, _w)

    def start_install():
        en = is_en()
        game_dir = Path(game_path_var.get().strip())
        if not is_valid_deltarune_dir(game_dir):
            messagebox.showwarning("Warning" if en else "Uyarı", "Please select a valid DELTARUNE folder." if en else "Lütfen geçerli bir DELTARUNE oyun klasörü seçin.")
            return

        m_menu = mod_vars["menu"].get()
        m_fps = mod_vars["fps60"].get()
        m_border = mod_vars["border"].get()
        if not (m_menu or m_fps or m_border):
            messagebox.showinfo("Info", "Please select at least 1 mod." if en else "Kurulum için en az 1 mod seçmelisiniz.")
            return

        selected_indices = [
            i for i, (_, _, _, rel, _) in enumerate(ROLE_INFO)
            if ch_vars[i].get() and (game_dir / rel).is_file()
        ]
        if not selected_indices:
            messagebox.showinfo("Info", "Please select at least one chapter." if en else "Lütfen uygulanacak en az bir bölüm seçin.")
            return

        base_dir = get_base_dir()
        r_type, r_cmd, r_desc = detect_utmt_runner(base_dir)
        if not r_type:
            messagebox.showerror("Error", "Please install 'wine' or 'dotnet' to run UTMT CLI on Linux.")
            return

        idx_lang = 1 if en else 0
        active_names = [titles[idx_lang] for k, titles, _, _ in MODS_INFO if mod_vars[k].get()]

        if cfg["confirm"]:
            msg = ("The following mods will be installed:\n\n• " if en else "Aşağıdaki modlar kurulacak:\n\n• ") + "\n• ".join(active_names)
            if not messagebox.askyesno("Confirm" if en else "Onay", msg):
                return

        set_working(True)
        txt_log.config(state="normal")
        txt_log.delete("1.0", "end")
        txt_log.config(state="disabled")

        def worker():
            append_log(("Selected Mods: " if en else "Seçili Modlar: ") + " + ".join(active_names))
            append_log(f"UTMT CLI: {r_desc}")

            backup_root = game_dir / "mod_backups"
            temp_dir = Path(tempfile.mkdtemp(prefix="dr_mod_installer_"))
            mods_dir = base_dir / "mods"
            patcher_csx = mods_dir / "patcher.csx"

            try:
                total_steps = len(selected_indices) * (2 if (m_fps and (m_border or m_menu)) else 1)
                step = 0

                for idx in selected_indices:
                    role_key, tr_title, en_title, rel_path, fps_script = ROLE_INFO[idx]
                    role_title = en_title if en else tr_title
                    target_win = game_dir / rel_path
                    backup_win = backup_root / rel_path
                    backup_win.parent.mkdir(parents=True, exist_ok=True)

                    if not backup_win.is_file():
                        append_log(f"[{role_title}] Backing up -> mod_backups/{rel_path}" if en else f"[{role_title}] Yedekleniyor -> mod_backups/{rel_path}")
                        shutil.copy2(target_win, backup_win)
                    else:
                        append_log(f"[{role_title}] Starting from clean backup..." if en else f"[{role_title}] Temiz yedek üzerinden kuruluma başlanıyor...")

                    curr_in = backup_win

                    if m_fps:
                        step += 1
                        set_progress(int(step * 100 / (total_steps + 1)), f"{role_title} • 60 FPS...")
                        fps_csx = mods_dir / "60fps" / fps_script
                        stage1_out = (temp_dir / f"{role_key}_stage1.win") if (m_border or m_menu) else target_win
                        append_log(f"[{role_title}] UTMT CLI -> {fps_script}")
                        ok = run_utmt_job(
                            base_dir, r_type, r_cmd, curr_in, stage1_out, fps_csx,
                            temp_dir, role_key, m_fps, m_border, m_menu, cfg["verbose"], append_log
                        )
                        if not ok:
                            raise RuntimeError(f"{role_title}: 60 FPS patch failed.")
                        curr_in = stage1_out

                    if m_border or m_menu:
                        step += 1
                        set_progress(int(step * 100 / (total_steps + 1)), f"{role_title} • Patching...")
                        append_log(f"[{role_title}] UTMT CLI -> patcher.csx")
                        ok = run_utmt_job(
                            base_dir, r_type, r_cmd, curr_in, target_win, patcher_csx,
                            temp_dir, role_key, m_fps, m_border, m_menu, cfg["verbose"], append_log
                        )
                        if not ok:
                            raise RuntimeError(f"{role_title}: Mod patch failed.")

                    append_log(f"[{role_title}] ✓ " + ("Completed" if en else "Tamamlandı"))

                set_progress(100, "✓ Installation complete" if en else "✓ Kurulum tamamlandı")
                append_log("All selected mods installed successfully!" if en else "Tüm seçili modlar başarıyla kuruldu!")
                root.after(
                    0,
                    lambda: messagebox.showinfo(
                        "Complete" if en else "Kurulum Tamamlandı",
                        ("Selected mods installed successfully!\n\n• " if en else "Seçtiğiniz modlar başarıyla kuruldu!\n\n• ")
                        + "\n• ".join(active_names),
                    ),
                )
            except Exception as ex:
                set_progress(0, "✗ Error" if en else "✗ Hata oluştu")
                append_log(f"ERROR: {ex}")
                root.after(0, lambda e=ex: messagebox.showerror("Error", str(e)))
            finally:
                shutil.rmtree(temp_dir, ignore_errors=True)
                try:
                    (mods_dir / "patch_config.txt").unlink(missing_ok=True)
                except Exception:
                    pass
                set_working(False)

        threading.Thread(target=worker, daemon=True).start()

    def start_restore():
        en = is_en()
        game_dir = Path(game_path_var.get().strip())
        backup_root = game_dir / "mod_backups"
        if not backup_root.is_dir():
            messagebox.showinfo("No Backup" if en else "Yedek Yok", "No 'mod_backups' folder found." if en else "Oyun klasöründe 'mod_backups' bulunamadı.")
            return
        if not messagebox.askyesno(
            "Restore Original" if en else "Orijinale Dön",
            "Restore original data.win files and remove all mods?" if en else "Orijinal data.win dosyaları geri yüklensin mi?",
        ):
            return

        set_working(True)
        restored = 0
        for _, tr_title, en_title, rel_path, _ in ROLE_INFO:
            r_title = en_title if en else tr_title
            b_win = backup_root / rel_path
            t_win = game_dir / rel_path
            if b_win.is_file():
                shutil.copy2(b_win, t_win)
                append_log(f"[{r_title}] ✓ Restored" if en else f"[{r_title}] ✓ Orijinal data.win geri yüklendi")
                restored += 1
        set_progress(100, f"✓ {restored} restored")
        set_working(False)
        messagebox.showinfo("Done", f"{restored} chapters restored." if en else f"{restored} bölüm orijinal haline döndürüldü.")

    btn_restore = tk.Button(
        btn_row,
        text="Orijinale Dön",
        font=("Sans", 10, "bold"),
        fg=TEXT_PRIMARY,
        bg=BG_CARD,
        activebackground=BG_CARD_SEL,
        activeforeground=TEXT_PRIMARY,
        bd=0,
        highlightthickness=1,
        highlightbackground=BORDER_SUBTLE,
        padx=22,
        pady=10,
        cursor="hand2",
        command=start_restore,
    )
    btn_restore.pack(side="left", padx=(0, 12))

    btn_install = tk.Button(
        btn_row,
        text="Seçili Modları Kur",
        font=("Sans", 10, "bold"),
        fg="#FFFFFF",
        bg=get_accent()[0],
        activebackground=get_accent()[1],
        activeforeground="#FFFFFF",
        bd=0,
        padx=24,
        pady=10,
        cursor="hand2",
        command=start_install,
    )
    btn_install.pack(side="left", fill="x", expand=True)

    def apply_localization():
        en = is_en()
        acc_c, acc_h, _ = get_accent()
        lbl_sub.config(text=f"Mod Installer  •  UTMT CLI ({runner_desc})" if en else f"Mod Kurulum Aracı  •  UTMT CLI ({runner_desc})")
        btn_lang.config(text="TR" if en else "EN")
        btn_settings.config(text="⚙ Settings" if en else "⚙ Ayarlar")
        btn_fs.config(
            text=("Exit Fullscreen (F11)" if en else "Küçült (F11)")
            if is_fullscreen[0]
            else ("Fullscreen (F11)" if en else "Tam Ekran (F11)")
        )
        lbl_path_cap.config(text="Game Directory" if en else "Oyun Klasörü")
        btn_browse.config(text="Browse..." if en else "Klasör Seç")
        refresh_cards_style()
        for i, (_, tr_n, en_n, _, _) in enumerate(ROLE_INFO):
            ch_checks[i].config(text=en_n if en else tr_n)
        if not is_working[0]:
            lbl_progress.config(text="Ready to install" if en else "Kuruluma hazır")
        prog_bg.itemconfig(prog_rect, fill=acc_c)
        btn_restore.config(text="Restore Original" if en else "Orijinale Dön")
        btn_install.config(text="Install Selected Mods" if en else "Seçili Modları Kur", bg=acc_c, activebackground=acc_h)
        validate_path()

    apply_localization()
    append_log(
        "Ready. Select your desired mod combination and click Install."
        if is_en()
        else "Hazır. İstediğiniz mod kombinasyonunu seçip kurulumu başlatabilirsiniz."
    )
    root.mainloop()


def run_cli():
    cfg = load_settings()
    en = cfg["lang"] == "en"
    print("====================================================")
    print("     DELTARUNE MOD INSTALLER (Linux CLI Mode)       ")
    print("====================================================")
    base_dir = get_base_dir()
    r_type, r_cmd, r_desc = detect_utmt_runner(base_dir)
    print(f"UTMT CLI Engine: {r_desc}")

    default_path = auto_detect_game_path()
    prompt_path = input(f"DELTARUNE Folder [{default_path}]: ").strip() or default_path
    game_dir = Path(prompt_path)
    if not is_valid_deltarune_dir(game_dir):
        print("ERROR: Valid DELTARUNE folder not found.")
        sys.exit(1)

    print("\nSelect mods to install (e.g. 1 2 3 or 1 3, or R to Restore Original):")
    print("  [1] Redesigned Menu & Turkish Mod / Yenilenmiş Türkçe Menü")
    print("  [2] 60 FPS Mod")
    print("  [3] Console Border Mod (NXRUNE)")
    print("  [R] Restore Original / Orijinale Dön")
    choice = input("Choice [1 2 3]: ").strip().upper() or "1 2 3"

    backup_root = game_dir / "mod_backups"
    if choice == "R":
        if not backup_root.is_dir():
            print("Backup folder (mod_backups) not found.")
            sys.exit(1)
        for _, tr_title, en_title, rel_path, _ in ROLE_INFO:
            b_win = backup_root / rel_path
            if b_win.is_file():
                shutil.copy2(b_win, game_dir / rel_path)
                print(f"[{en_title if en else tr_title}] ✓ Restored.")
        sys.exit(0)

    m_menu = "1" in choice
    m_fps = "2" in choice
    m_border = "3" in choice
    temp_dir = Path(tempfile.mkdtemp(prefix="dr_mod_installer_"))
    mods_dir = base_dir / "mods"

    try:
        for role_key, tr_title, en_title, rel_path, fps_script in ROLE_INFO:
            role_title = en_title if en else tr_title
            target_win = game_dir / rel_path
            if not target_win.is_file():
                continue
            backup_win = backup_root / rel_path
            backup_win.parent.mkdir(parents=True, exist_ok=True)
            if not backup_win.is_file():
                shutil.copy2(target_win, backup_win)
            curr_in = backup_win
            if m_fps:
                s1 = (temp_dir / f"{role_key}_s1.win") if (m_border or m_menu) else target_win
                if not run_utmt_job(base_dir, r_type, r_cmd, curr_in, s1, mods_dir / "60fps" / fps_script, temp_dir, role_key, m_fps, m_border, m_menu, cfg["verbose"], print):
                    raise RuntimeError(f"{role_title} 60 FPS error")
                curr_in = s1
            if m_border or m_menu:
                if not run_utmt_job(base_dir, r_type, r_cmd, curr_in, target_win, mods_dir / "patcher.csx", temp_dir, role_key, m_fps, m_border, m_menu, cfg["verbose"], print):
                    raise RuntimeError(f"{role_title} patch error")
            print(f"[{role_title}] ✓ Completed")
    finally:
        shutil.rmtree(temp_dir, ignore_errors=True)


if __name__ == "__main__":
    if "--cli" in sys.argv:
        run_cli()
    else:
        try:
            import tkinter  # noqa: F401
            if os.name != "nt" and not os.environ.get("DISPLAY") and not os.environ.get("WAYLAND_DISPLAY"):
                run_cli()
            else:
                run_gui()
        except ImportError:
            run_cli()
