_t += (1 / global.fps_scale);
_alpha = lerp(_alpha, 1, 1 - power(0.965, 1 / global.fps_scale));
var th = 1;
if (variable_global_exists("menu_theme"))
{
    th = global.menu_theme;
}
var nr = 255;
var ng = 214;
var nb = 150;
switch (th)
{
    case 2:
        nr = 96;
        ng = 220;
        nb = 255;
        break;
    case 3:
        nr = 198;
        ng = 150;
        nb = 255;
        break;
    case 4:
        nr = 120;
        ng = 235;
        nb = 200;
        break;
    case 5:
        nr = 255;
        ng = 140;
        nb = 100;
        break;
    case 6:
        nr = 150;
        ng = 158;
        nb = 185;
        break;
    case 7:
        nr = 120;
        ng = 130;
        nb = 165;
        break;
    default:
        nr = 255;
        ng = 214;
        nb = 150;
        break;
}
var _lr = 1 - power(0.94, 1 / global.fps_scale);
_tr = lerp(_tr, nr, _lr);
_tg = lerp(_tg, ng, _lr);
_tb = lerp(_tb, nb, _lr);
var h = room_height;
for (var i = 0; i < _scount; i++)
{
    _sy[i] += ((0.15 + (_sz[i] * 0.55)) * (1 + _sz[i])) / global.fps_scale;
    if (_sy[i] > (h + 4))
    {
        _sy[i] -= h + 8;
        _sx[i] = random(room_width);
    }
}
