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
draw_set_color(make_color_rgb(110, 240, 210));
draw_set_alpha(_alpha * 0.11);
for (var a = 0; a < 4; a++)
{
    var ax = 50 + (a * 74);
    draw_rectangle(ax - 14, 28, ax - 13, 195, false);
    draw_rectangle(ax + 13, 28, ax + 14, 195, false);
    draw_rectangle(ax - 14, 28, ax + 14, 29, false);
    draw_rectangle(ax, 18, ax + 1, 195, false);
}
for (var r = 0; r < 3; r++)
{
    var rx = 68 + (r * 86) + (sin((_t * 0.015) + r) * 10);
    draw_set_color(make_color_rgb(150, 255, 225));
    draw_set_alpha(_alpha * 0.05);
    for (var k = 0; k < 16; k++)
    {
        var ky = k * 15;
        draw_rectangle((rx + (k * 2)) - 12, ky, rx + (k * 2) + 12, ky + 15, false);
    }
}
for (var i = 0; i < _scount; i++)
{
    var px = floor(_sx[i]);
    var py = floor(_sy[i]);
    draw_set_color(make_color_rgb(165, 245, 235));
    draw_set_alpha(_alpha * (0.2 + (0.45 * _sz[i])));
    draw_rectangle(px, py, px, py + 4 + round(_sz[i] * 4), false);
}
for (var rp = 0; rp < 4; rp++)
{
    var rphase = frac((_t * 0.012) + (rp * 0.25));
    var rx = 55 + (rp * 70);
    var ry = 216 + ((rp % 2) * 8);
    var rw_rad = round(rphase * 18);
    draw_set_color(make_color_rgb(130, 245, 220));
    draw_set_alpha(_alpha * (1 - rphase) * 0.28);
    draw_rectangle(rx - rw_rad, ry, rx + rw_rad, ry + 1, false);
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
