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
var sw1 = sin(_t * 0.026) * 70;
var sw2 = cos(_t * 0.022) * 70;
for (var s = 0; s < 20; s++)
{
    var f = s / 20;
    var sy = f * rh;
    var sx1 = 64 + (sw1 * f);
    var sx2 = 256 + (sw2 * f);
    var hw = 8 + (f * 36);
    draw_set_color(make_color_rgb(255, 225, 120));
    draw_set_alpha(_alpha * 0.045 * (1 - (f * 0.25)));
    draw_rectangle(sx1 - hw, sy, sx1 + hw, sy + 12, false);
    draw_set_color(make_color_rgb(230, 110, 255));
    draw_rectangle(sx2 - hw, sy, sx2 + hw, sy + 12, false);
}
draw_set_color(make_color_rgb(18, 5, 32));
draw_set_alpha(_alpha * 0.85);
draw_rectangle(0, 206, rw, rh, false);
draw_rectangle(128, 178, 192, 206, false);
draw_set_color(make_color_rgb(235, 140, 255));
draw_set_alpha(_alpha * 0.28);
draw_line_width(160, 178, 142, 160, 1);
draw_line_width(160, 178, 178, 160, 1);
draw_rectangle(133, 182, 187, 202, false);
draw_set_color(make_color_rgb(220, 160, 255));
draw_set_alpha(_alpha * 0.045);
for (var sy = 0; sy < rh; sy += 3)
{
    draw_rectangle(0, sy, rw, sy, false);
}
for (var i = 0; i < _scount; i++)
{
    var tw = 0.35 + (0.65 * (0.5 + (0.5 * sin((_t * 0.045) + _sph[i]))));
    var px = floor(_sx[i]);
    var py = floor(_sy[i]);
    var is_gold = (i % 3) == 0;
    draw_set_color(is_gold ? make_color_rgb(255, 230, 130) : make_color_rgb(235, 155, 255));
    draw_set_alpha(_alpha * tw * (0.3 + (0.65 * _sz[i])));
    if (_sz[i] > 0.65)
    {
        draw_rectangle(px - 1, py, px + 1, py, false);
        draw_rectangle(px, py - 1, px, py + 1, false);
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
