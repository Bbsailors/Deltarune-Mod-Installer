var ha = _head_alpha;
var last = array_length(_chapters) - 1;
var pulse = 0.5 + (0.5 * sin(current_time * 0.004));
draw_set_halign(fa_center);
draw_set_valign(fa_top);
draw_set_font(fnt_mainbig);
draw_set_color(c_black);
draw_set_alpha(ha * 0.55);
draw_text_transformed(322, 24, "BÖLÜM SEÇİMİ", 1.25, 1.25, 0);
draw_set_color(make_color_rgb(236, 241, 255));
draw_set_alpha(ha);
draw_text_transformed(320, 22, "BÖLÜM SEÇİMİ", 1.25, 1.25, 0);
draw_set_color(make_color_rgb(255, 214, 150));
draw_set_alpha(ha * 0.65);
draw_rectangle(250, 56, 390, 57, false);
draw_set_font(fnt_main);
draw_set_color(make_color_rgb(150, 160, 200));
draw_set_alpha(ha * 0.85);
draw_text_transformed(320, 62, "SEÇMEK İÇİN  <  >  KULLANIN", 0.8, 0.8, 0);
if (_chapter_index > 0)
{
    draw_set_color(make_color_rgb(255, 214, 150));
    draw_set_alpha(ha * (0.35 + (0.4 * pulse)));
    for (var k = 0; k < 7; k++)
    {
        draw_rectangle(20 + k, 240 - k, 22 + k, 240 + k, false);
    }
}
if (_chapter_index < last)
{
    draw_set_color(make_color_rgb(255, 214, 150));
    draw_set_alpha(ha * (0.35 + (0.4 * pulse)));
    for (var k = 0; k < 7; k++)
    {
        draw_rectangle(618 - k, 240 - k, 620 - k, 240 + k, false);
    }
}
var n = array_length(_chapters);
var dw = 14;
var startx = 320 - ((n - 1) * dw * 0.5);
for (var i = 0; i < n; i++)
{
    var on = i == _chapter_index;
    draw_set_color(on ? make_color_rgb(255, 214, 150) : make_color_rgb(58, 66, 100));
    draw_set_alpha(ha * (on ? 1 : 0.8));
    if (on)
    {
        draw_rectangle((startx + (i * dw)) - 3, 402, startx + (i * dw) + 3, 409, false);
    }
    else
    {
        draw_rectangle((startx + (i * dw)) - 2, 404, startx + (i * dw) + 2, 407, false);
    }
}
draw_set_alpha(1);
draw_set_color(c_white);
draw_set_halign(fa_left);
draw_set_valign(fa_top);
