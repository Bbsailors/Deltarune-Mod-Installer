if (_alpha <= 0.001)
{
    exit;
}
var a = _alpha;
var w = _card_w;
var h = _card_h;
var px = x;
var py = y;
var sel = _is_highlighted;
var pulse = 0.5 + (0.5 * sin(current_time * 0.004));
if (sel)
{
    draw_sprite_ext(spr_menu_glow, 0, px + (w * 0.5), py + (h * 0.5), 2.4, 3, 0, make_color_rgb(_th_r, _th_g, _th_b), a * (0.14 + (0.08 * pulse)));
}
draw_set_color(c_black);
draw_set_alpha(a * 0.35);
draw_rectangle(px + 4, py + 6, px + w + 4, py + h + 6, false);
for (var i = 0; i < 12; i++)
{
    var f = i / 12;
    var rr = round((26 - (16 * f)) + (_th_r * 0.05));
    var gg = round((28 - (18 * f)) + (_th_g * 0.05));
    var bb = round((48 - (28 * f)) + (_th_b * 0.05));
    draw_set_color(rr + (gg * #000100) + (bb * #000001));
    draw_set_alpha(a * 0.96);
    draw_rectangle(px, py + (h * (i / 12)), px + w, py + (h * ((i + 1) / 12)), false);
}
draw_set_color(make_color_rgb(_th_r, _th_g, _th_b));
draw_set_alpha(a * 0.1);
draw_rectangle(px + 2, py + 3, (px + w) - 2, py + 62, false);
var bc = sel ? make_color_rgb(_th_r, _th_g, _th_b) : make_color_rgb(56, 64, 94);
draw_set_color(bc);
draw_set_alpha(a * (sel ? 0.95 : 0.8));
draw_rectangle(px, py, px + w, py + 3, false);
draw_rectangle(px, (py + h) - 3, px + w, py + h, false);
draw_rectangle(px, py, px + 2, py + h, false);
draw_rectangle((px + w) - 2, py, px + w, py + h, false);
var icon_col = sel ? 16777215 : make_color_rgb(200, 210, 235);
draw_sprite_ext(spr_chapterIcon, _icon_index, (px + (w * 0.5)) - 23, py + 18, 1.8, 1.8, 0, icon_col, a);
draw_set_halign(fa_center);
draw_set_valign(fa_top);
draw_set_font(fnt_main);
draw_set_color(make_color_rgb(_th_r, _th_g, _th_b));
draw_set_alpha(a * 0.95);
draw_text_transformed(px + (w * 0.5), py + 66, "BÖLÜM", 0.9, 0.9, 0);
draw_set_font(fnt_mainbig);
draw_set_color(c_black);
draw_set_alpha(a * 0.5);
draw_text_transformed(px + (w * 0.5) + 2, py + 80, string(_chapter), 2.2, 2.2, 0);
draw_set_color(c_white);
draw_set_alpha(a);
draw_text_transformed(px + (w * 0.5), py + 78, string(_chapter), 2.2, 2.2, 0);
draw_set_font(_font);
draw_set_color(c_black);
draw_set_alpha(a * 0.5);
draw_text_transformed(px + (w * 0.5) + 1, py + 141, _chapter_title, 1, 1, 0);
draw_set_color(sel ? make_color_rgb(255, 248, 225) : make_color_rgb(214, 222, 244));
draw_set_alpha(a);
draw_text_transformed(px + (w * 0.5), py + 140, _chapter_title, 1, 1, 0);
for (var i = 0; i < 3; i++)
{
    draw_sprite_ext(spr_ui_star, _completed_files[i], ((px + (w * 0.5)) - 24) + (i * 16), py + 163, 1, 1, 0, c_white, a * 0.9);
}
draw_set_color(make_color_rgb(_th_r, _th_g, _th_b));
draw_set_alpha(a * 0.3);
draw_rectangle(px + 16, py + 186, (px + w) - 16, py + 187, false);
draw_set_font(fnt_main);
if (!_has_data)
{
    draw_set_color(make_color_rgb(255, 92, 92));
    draw_set_alpha(a);
    draw_text_transformed(px + (w * 0.5), py + 206, "data.win", 1.05, 1.05, 0);
    draw_set_alpha(a * 0.95);
    draw_text_transformed(px + (w * 0.5), py + 222, "bulunamadı", 1.05, 1.05, 0);
}
else if (sel)
{
    draw_set_color(make_color_rgb(_th_r, _th_g, _th_b));
    draw_set_alpha(a * (0.45 + (0.55 * pulse)));
    draw_set_font(fnt_mainbig);
    draw_text_transformed(px + (w * 0.5), py + (h - 34), "Z'YE BAS", 0.75, 0.75, 0);
}
draw_set_color(make_color_rgb(255, 214, 150));
draw_set_alpha(a * (sel ? 0.55 : 0.25));
draw_rectangle(px + 10, (py + h) - 12, px + 24, (py + h) - 11, false);
draw_rectangle(px + 10, (py + h) - 12, px + 11, (py + h) - 4, false);
draw_rectangle((px + w) - 24, (py + h) - 12, (px + w) - 10, (py + h) - 11, false);
draw_rectangle((px + w) - 11, (py + h) - 12, (px + w) - 10, (py + h) - 4, false);
draw_set_alpha(1);
draw_set_color(c_white);
draw_set_halign(fa_left);
draw_set_valign(fa_top);
