depth = 90;
_alpha = 0;
_t = random(1000);
_rw = room_width;
_rh = room_height;
_tr = 255;
_tg = 214;
_tb = 150;
if (!variable_global_exists("menu_theme"))
{
    global.menu_theme = 1;
}
_scount = 140;
_sx = [];
_sy = [];
_sz = [];
_sph = [];
for (var i = 0; i < _scount; i++)
{
    _sx[i] = random(_rw);
    _sy[i] = random(_rh);
    _sz[i] = random_range(0.15, 1);
    _sph[i] = random(6.28318);
}
