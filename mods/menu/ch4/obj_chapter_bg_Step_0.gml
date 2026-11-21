_t += (1 / global.fps_scale);
_alpha = lerp(_alpha, 1, 1 - power(0.96, 1 / global.fps_scale));
var h = 240;
for (var i = 0; i < _scount; i++)
{
    _sy[i] += (0.9 + (_sz[i] * 1.4)) / global.fps_scale;
    if (_sy[i] > (h + 6))
    {
        _sy[i] -= h + 12;
        _sx[i] = random(320);
    }
}
