if (_alpha <= 0.001)
{
    exit;
}
var rw = 320;
var rh = 240;
var band = 4;
for (var yy = 0; yy < rh; yy += band)
{
    var f = yy / rh;
    var rr = 0;
    var gg = 0;
    var bb = 0;
    if (f < 0.5)
    {
        var t = f * 2;
        rr = _r1 + ((_r2 - _r1) * t);
        gg = _g1 + ((_g2 - _g1) * t);
        bb = _b1 + ((_b2 - _b1) * t);
    }
    else
    {
        var t = (f - 0.5) * 2;
        rr = _r2 + ((_r3 - _r2) * t);
        gg = _g2 + ((_g3 - _g2) * t);
        bb = _b2 + ((_b3 - _b2) * t);
    }
    draw_set_color(make_color_rgb(round(rr), round(gg), round(bb)));
    draw_set_alpha(_alpha);
    draw_rectangle(0, yy, rw, yy + band, false);
}
var hglow = 0.08 + (0.03 * sin(_t * 0.03));
draw_set_color(make_color_rgb(255, 160, 80));
draw_set_alpha(_alpha * hglow);
draw_circle(160, 210, 90, false);
draw_set_color(make_color_rgb(10, 3, 5));
draw_set_alpha(_alpha * 0.9);
draw_rectangle(0, 218, rw, rh, false);
draw_triangle(35, 218, 65, 194, 95, 218, false);
draw_triangle(220, 218, 255, 190, 290, 218, false);
var fwx = 160;
var fwy = 196;
var fwr = 24;
draw_set_color(make_color_rgb(255, 160, 95));
draw_set_alpha(_alpha * 0.16);
draw_circle(fwx, fwy, fwr, true);
draw_circle(fwx, fwy, fwr - 4, true);
for (var sp = 0; sp < 8; sp++)
{
    var ang = (_t * 0.008) + (sp * 0.785398);
    var sx = fwx + (cos(ang) * fwr);
    var sy = fwy + (sin(ang) * fwr);
    draw_line(fwx, fwy, sx, sy);
    draw_rectangle(sx - 1, sy - 1, sx + 1, sy + 1, false);
}
for (var s = 0; s < 2; s++)
{
    var ybase = 12 + (s * 18);
    for (var lx = 8; lx < rw; lx += 16)
    {
        var sag = sin((lx / rw) * 3.14159) * (10 + (s * 5));
        var ly = ybase + sag + (sin((_t * 0.03) + (lx * 0.04) + s) * 1.5);
        draw_set_color(make_color_rgb(80, 35, 30));
        draw_set_alpha(_alpha * 0.45);
        draw_rectangle(lx - 8, ly - 2, lx + 8, ly - 1, false);
        var b_pulse = 0.55 + (0.45 * sin((_t * 0.07) + (lx * 0.2) + (s * 2)));
        draw_set_color(make_color_rgb(255, 195, 100));
        draw_set_alpha(_alpha * 0.18 * b_pulse);
        draw_circle(lx, ly + 1, 5, false);
        draw_set_color((((lx div 16) % 2) == 0) ? make_color_rgb(255, 225, 130) : make_color_rgb(255, 120, 85));
        draw_set_alpha(_alpha * 0.65 * b_pulse);
        draw_circle(lx, ly + 1, 2, false);
    }
}
for (var i = 0; i < _scount; i++)
{
    var tw = 0.45 + (0.55 * (0.5 + (0.5 * sin((_t * 0.04) + _sph[i]))));
    var px = floor(_sx[i]);
    var py = floor(_sy[i]);
    if (_sz[i] > 0.6)
    {
        draw_set_color(make_color_rgb(255, 150, 70));
        draw_set_alpha(_alpha * tw * 0.2);
        draw_circle(px + 1, py + 1, 4, false);
        draw_set_color(make_color_rgb(255, 215, 130));
        draw_set_alpha(_alpha * tw * 0.75);
        draw_rectangle(px, py, px + 2, py + 3, false);
    }
    else
    {
        draw_set_color(make_color_rgb(255, 180, 110));
        draw_set_alpha(_alpha * tw * (0.3 + (0.5 * _sz[i])));
        draw_rectangle(px, py, px + 1, py + 1, false);
    }
}
draw_set_font(fnt_main);
draw_set_halign(fa_center);
draw_set_valign(fa_middle);
draw_set_color(make_color_rgb(_tr, _tg, _tb));
draw_set_alpha(_alpha * 0.07);
draw_text_transformed(160, 112, "FESTIVAL NIGHT", 1.8, 1.8, 0);
var gp = 0.28 + (0.1 * sin(_t * 0.03));
var m = 4;
var L = 16;
draw_set_color(make_color_rgb(_tr, _tg, _tb));
draw_set_alpha(_alpha * gp);
draw_rectangle(m, m, m + L, m + 1, false);
draw_rectangle(m, m, m + 1, m + L, false);
draw_rectangle(rw - m - L, m, rw - m, m + 1, false);
draw_rectangle(rw - m - 1, m, rw - m, m + L, false);
draw_rectangle(m, rh - m - 1, m + L, rh - m, false);
draw_rectangle(m, rh - m - L, m + 1, rh - m, false);
draw_rectangle(rw - m - L, rh - m - 1, rw - m, rh - m, false);
draw_rectangle(rw - m - 1, rh - m - L, rw - m, rh - m, false);
draw_set_alpha(1);
draw_set_color(c_white);
draw_set_halign(fa_left);
draw_set_valign(fa_top);
