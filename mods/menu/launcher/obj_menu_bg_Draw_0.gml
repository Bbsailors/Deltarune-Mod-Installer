if (_alpha <= 0.001)
{
    exit;
}
var rw = room_width;
var rh = room_height;
var th = 1;
if (variable_global_exists("menu_theme"))
{
    th = global.menu_theme;
}
var r1 = round(4 + (_tr * 0.05));
var g1 = round(3 + (_tg * 0.04));
var b1 = round(10 + (_tb * 0.06));
var r2 = round(8 + (_tr * 0.2));
var g2 = round(6 + (_tg * 0.2));
var b2 = round(18 + (_tb * 0.2));
var r3 = 3;
var g3 = 2;
var b3 = 8;
var band = 6;
for (var yy = 0; yy < rh; yy += band)
{
    var f = yy / rh;
    var rr = 0;
    var gg = 0;
    var bb = 0;
    if (f < 0.55)
    {
        var t = f / 0.55;
        rr = r1 + ((r2 - r1) * t);
        gg = g1 + ((g2 - g1) * t);
        bb = b1 + ((b2 - b1) * t);
    }
    else
    {
        var t = (f - 0.55) / 0.45;
        rr = r2 + ((r3 - r2) * t);
        gg = g2 + ((g3 - g2) * t);
        bb = b2 + ((b3 - b2) * t);
    }
    draw_set_color(make_color_rgb(round(rr), round(gg), round(bb)));
    draw_set_alpha(_alpha);
    draw_rectangle(0, yy, rw, yy + band, false);
}
var n0 = make_color_rgb(round(20 + (_tr * 0.5)), round(10 + (_tg * 0.4)), round(60 + (_tb * 0.5)));
var n1 = make_color_rgb(round(15 + (_tr * 0.25)), round(35 + (_tg * 0.3)), round(110 + (_tb * 0.35)));
var n2 = make_color_rgb(round(40 + (_tr * 0.55)), round(8 + (_tg * 0.2)), round(30 + (_tb * 0.35)));
draw_sprite_ext(spr_menu_glow, 0, rw * 0.18, rh * 0.6, 3, 3, 0, n0, _alpha * 0.26);
draw_sprite_ext(spr_menu_glow, 0, rw * 0.84, rh * 0.26, 2.6, 2.6, 0, n1, _alpha * 0.22);
draw_sprite_ext(spr_menu_glow, 0, rw * 0.5, rh * 0.5, 3.2, 3.2, 0, n2, _alpha * 0.2);
if (th == 1)
{
    var fw = 36 + (sin(_t * 0.04) * 6);
    draw_set_color(make_color_rgb(220, 60, 110));
    draw_set_alpha(_alpha * 0.08);
    draw_rectangle(320 - fw, 0, 320 + fw, rh, false);
    draw_set_color(make_color_rgb(255, 215, 140));
    draw_set_alpha(_alpha * 0.12);
    draw_rectangle(320 - (fw * 0.45), 0, 320 + (fw * 0.45), rh, false);
    draw_set_color(make_color_rgb(16, 4, 12));
    draw_set_alpha(_alpha * 0.85);
    draw_rectangle(250, 390, 390, rh, false);
    draw_rectangle(230, 410, 250, rh, false);
    draw_rectangle(390, 410, 410, rh, false);
    draw_rectangle(275, 360, 365, 390, false);
    draw_rectangle(305, 332, 335, 360, false);
    draw_set_color(make_color_rgb(255, 214, 150));
    draw_set_alpha(_alpha * 0.28);
    for (var d = 0; d < 10; d++)
    {
        draw_rectangle(320 - (10 - d), 350 - d, 320 + (10 - d), 350 + d, false);
    }
}
else if (th == 2)
{
    var hy = 340;
    draw_set_color(make_color_rgb(6, 22, 48));
    draw_set_alpha(_alpha * 0.9);
    for (var bx = 0; bx < rw; bx += 32)
    {
        var bh = 28 + (((bx * 37) % 7) * 12);
        draw_rectangle(bx, hy - bh, bx + 28, hy, false);
        draw_set_color(make_color_rgb(80, 235, 255));
        draw_set_alpha(_alpha * 0.22);
        draw_rectangle(bx, hy - bh, bx + 28, (hy - bh) + 2, false);
        var wy = (hy - bh) + 6;
        while (wy < (hy - 4))
        {
            draw_rectangle(bx + 5, wy, bx + 10, wy + 3, false);
            draw_rectangle(bx + 16, wy, bx + 21, wy + 3, false);
            wy += 8;
        }
        draw_set_color(make_color_rgb(6, 22, 48));
        draw_set_alpha(_alpha * 0.9);
    }
    draw_set_color(make_color_rgb(4, 12, 28));
    draw_set_alpha(_alpha * 0.95);
    draw_rectangle(0, hy, rw, rh, false);
    draw_set_color(make_color_rgb(80, 240, 255));
    draw_set_alpha(_alpha * 0.32);
    var goff = (_t * 0.6) % 18;
    var gy = hy + goff;
    while (gy < rh)
    {
        draw_rectangle(0, gy, rw, gy + 1, false);
        gy += 18;
    }
    for (var gx = -160; gx <= (rw + 160); gx += 48)
    {
        draw_line_width(320 + ((gx - 320) * 0.25), hy, gx, rh, 1);
    }
}
else if (th == 3)
{
    var sw1 = sin(_t * 0.025) * 140;
    var sw2 = cos(_t * 0.021) * 140;
    for (var s = 0; s < 24; s++)
    {
        var f = s / 24;
        var sy = f * rh;
        var sx1 = 120 + (sw1 * f);
        var sx2 = 520 + (sw2 * f);
        var hw = 14 + (f * 70);
        draw_set_color(make_color_rgb(255, 220, 120));
        draw_set_alpha(_alpha * 0.04 * (1 - (f * 0.3)));
        draw_rectangle(sx1 - hw, sy, sx1 + hw, sy + 20, false);
        draw_set_color(make_color_rgb(220, 110, 255));
        draw_rectangle(sx2 - hw, sy, sx2 + hw, sy + 20, false);
    }
    draw_set_color(make_color_rgb(210, 150, 255));
    draw_set_alpha(_alpha * 0.045);
    for (var sy = 0; sy < rh; sy += 4)
    {
        draw_rectangle(0, sy, rw, sy + 1, false);
    }
}
else if (th == 4)
{
    draw_set_color(make_color_rgb(110, 240, 210));
    draw_set_alpha(_alpha * 0.09);
    for (var a = 0; a < 5; a++)
    {
        var ax = 80 + (a * 120);
        draw_rectangle(ax - 22, 60, ax - 20, 380, false);
        draw_rectangle(ax + 20, 60, ax + 22, 380, false);
        draw_rectangle(ax - 22, 60, ax + 22, 62, false);
    }
    for (var r = 0; r < 4; r++)
    {
        var rx = 110 + (r * 130) + (sin((_t * 0.015) + r) * 18);
        draw_set_color(make_color_rgb(140, 255, 225));
        draw_set_alpha(_alpha * 0.045);
        for (var k = 0; k < 20; k++)
        {
            var ky = k * 24;
            draw_rectangle((rx + (k * 4)) - 18, ky, rx + (k * 4) + 18, ky + 24, false);
        }
    }
}
else if (th == 5)
{
    for (var g = 0; g < 2; g++)
    {
        var base_y = 28 + (g * 34);
        for (var lx = 20; lx < rw; lx += 36)
        {
            var sag = sin((lx / rw) * 3.14159) * 26;
            var by = base_y + sag;
            var pulse = 0.65 + (0.35 * sin((_t * 0.06) + (lx * 0.05) + g));
            var bcol = (((lx div 36) % 2) == 0) ? make_color_rgb(255, 210, 110) : make_color_rgb(255, 110, 85);
            draw_set_color(bcol);
            draw_set_alpha(_alpha * 0.22 * pulse);
            draw_rectangle(lx - 5, by - 5, lx + 5, by + 5, false);
            draw_set_alpha(_alpha * 0.7 * pulse);
            draw_rectangle(lx - 2, by - 2, lx + 2, by + 2, false);
        }
    }
}
else
{
    draw_set_color(make_color_rgb(round(_tr), round(_tg), round(_tb)));
    for (var c = 1; c <= 4; c++)
    {
        var rad = (c * 55) + ((_t * 0.5) % 55);
        draw_set_alpha(_alpha * 0.08 * (1 - (rad / 260)));
        draw_rectangle(320 - rad, 240 - (rad * 0.6), 320 + rad, (240 - (rad * 0.6)) + 2, false);
        draw_rectangle(320 - rad, 240 + (rad * 0.6), 320 + rad, 240 + (rad * 0.6) + 2, false);
    }
}
for (var i = 0; i < _scount; i++)
{
    var tw = 0.35 + (0.65 * (0.5 + (0.5 * sin((_t * 0.035) + _sph[i]))));
    var px = floor(_sx[i]);
    var py = floor(_sy[i]);
    var big = (_sz[i] > 0.72) ? 2 : 1;
    draw_set_color(make_color_rgb(round(160 + (_tr * 0.35)), round(160 + (_tg * 0.35)), round(160 + (_tb * 0.35))));
    draw_set_alpha(_alpha * tw * (0.3 + (0.7 * _sz[i])));
    if (th == 4)
    {
        draw_rectangle(px, py, px + 1, py + 7 + (big * 3), false);
    }
    else if (th == 2)
    {
        draw_rectangle(px, py, px + 2, py + 4, false);
    }
    else if (th == 5)
    {
        draw_rectangle(px - 1, py - 2, px + 2, py + 2, false);
    }
    else
    {
        draw_rectangle(px, py, px + big, py + big, false);
    }
}
if (!instance_exists(obj_screen_start))
{
    draw_set_font(fnt_mainbig);
    draw_set_halign(fa_center);
    draw_set_color(make_color_rgb(round(_tr), round(_tg), round(_tb)));
    draw_set_alpha(_alpha * 0.06);
    draw_text_transformed(rw * 0.5, (rh * 0.5) - 40, "DELTARUNE", 3.2, 3.2, 0);
    draw_set_halign(fa_left);
}
for (var i = 0; i < 22; i++)
{
    var va = 0.048 * (1 - (i / 22));
    draw_set_color(c_black);
    draw_set_alpha(_alpha * va);
    draw_rectangle(0, rh - ((i + 1) * 4), rw, rh - (i * 4), false);
}
var gp = 0.22 + (0.1 * sin(_t * 0.03));
var m = 10;
var L = 44;
var cc = make_color_rgb(round(90 + (_tr * 0.6)), round(80 + (_tg * 0.6)), round(80 + (_tb * 0.6)));
draw_set_color(cc);
draw_set_alpha(_alpha * gp);
draw_rectangle(m, m, m + L, m + 2, false);
draw_rectangle(m, m, m + 2, m + L, false);
draw_rectangle(rw - m - L, m, rw - m, m + 2, false);
draw_rectangle(rw - m - 2, m, rw - m, m + L, false);
draw_rectangle(m, rh - m - 2, m + L, rh - m, false);
draw_rectangle(m, rh - m - L, m + 2, rh - m, false);
draw_rectangle(rw - m - L, rh - m - 2, rw - m, rh - m, false);
draw_rectangle(rw - m - 2, rh - m - L, rw - m, rh - m, false);
draw_set_color(make_color_rgb(round(40 + (_tr * 0.4)), round(60 + (_tg * 0.4)), round(120 + (_tb * 0.4))));
draw_set_alpha(_alpha * 0.1);
draw_rectangle(0, rh - 3, rw, rh, false);
draw_set_alpha(1);
draw_set_color(c_white);
draw_set_halign(fa_left);
