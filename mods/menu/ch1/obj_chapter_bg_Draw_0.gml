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
    if (f < 0.55)
    {
        var t = f / 0.55;
        rr = _r1 + ((_r2 - _r1) * t);
        gg = _g1 + ((_g2 - _g1) * t);
        bb = _b1 + ((_b2 - _b1) * t);
    }
    else
    {
        var t = (f - 0.55) / 0.45;
        rr = _r2 + ((_r3 - _r2) * t);
        gg = _g2 + ((_g3 - _g2) * t);
        bb = _b2 + ((_b3 - _b2) * t);
    }
    draw_set_color(make_color_rgb(round(rr), round(gg), round(bb)));
    draw_set_alpha(_alpha);
    draw_rectangle(0, yy, rw, yy + band, false);
}
var fw = 22 + (sin(_t * 0.04) * 4);
draw_set_color(make_color_rgb(210, 45, 95));
draw_set_alpha(_alpha * 0.09);
draw_rectangle(160 - fw, 0, 160 + fw, rh, false);
draw_set_color(make_color_rgb(255, 210, 140));
draw_set_alpha(_alpha * 0.13);
draw_rectangle(160 - (fw * 0.45), 0, 160 + (fw * 0.45), rh, false);
for (var a = 0; a < 3; a++)
{
    var base = 54 + (a * 46);
    var amp = 10 + (a * 4);
    var spd = 0.006 + (a * 0.002);
    draw_set_color(make_color_rgb(235, 90, 130));
    draw_set_alpha(_alpha * 0.07);
    for (var xx = 0; xx < rw; xx += 6)
    {
        var ay = base + (sin((xx * 0.022) + (_t * spd) + (a * 2)) * amp);
        draw_rectangle(xx, ay, xx + 6, ay + 2, false);
    }
}
draw_set_color(make_color_rgb(14, 3, 9));
draw_set_alpha(_alpha * 0.85);
draw_rectangle(118, 192, 202, rh, false);
draw_rectangle(104, 204, 118, rh, false);
draw_rectangle(202, 204, 216, rh, false);
draw_rectangle(134, 174, 186, 192, false);
draw_rectangle(150, 158, 170, 174, false);
draw_set_color(make_color_rgb(255, 210, 140));
draw_set_alpha(_alpha * 0.25);
for (var d = 0; d < 6; d++)
{
    draw_rectangle(160 - (6 - d), 170 - d, 160 + (6 - d), 170 + d, false);
}
for (var i = 0; i < _scount; i++)
{
    var tw = 0.4 + (0.6 * (0.5 + (0.5 * sin((_t * 0.04) + _sph[i]))));
    var px = floor(_sx[i]);
    var py = floor(_sy[i]);
    var is_gold = (i % 3) == 0;
    draw_set_color(is_gold ? make_color_rgb(255, 215, 140) : make_color_rgb(245, 95, 135));
    draw_set_alpha(_alpha * tw * (0.35 + (0.65 * _sz[i])));
    if (_sz[i] > 0.65)
    {
        draw_rectangle(px - 1, py, px + 1, py + 1, false);
        draw_rectangle(px, py - 1, px, py + 2, false);
    }
    else
    {
        draw_rectangle(px, py, px + 1, py + 1, false);
    }
}
draw_set_font(fnt_mainbig);
draw_set_halign(fa_center);
draw_set_valign(fa_top);
draw_set_color(make_color_rgb(_tr, _tg, _tb));
draw_set_alpha(_alpha * 0.07);
draw_text_transformed(160, 102, _title, 1.5, 1.5, 0);
var gp = 0.24 + (0.1 * sin(_t * 0.03));
var m = 4;
var L = 18;
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
