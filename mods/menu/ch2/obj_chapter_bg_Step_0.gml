_t += (1 / global.fps_scale);
_alpha = lerp(_alpha, 1, 1 - power(0.96, 1 / global.fps_scale));
var h = 240;
for (var i = 0; i < _scount; i++)
{
    _sy[i] -= (0.35 + (_sz[i] * 0.75)) / global.fps_scale;
    if (_sy[i] < -6)
    {
        _sy[i] += h + 10;
        _sx[i] = random(320);
    }
}
