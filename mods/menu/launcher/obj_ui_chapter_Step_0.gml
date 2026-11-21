if (_fade_in)
{
    _alpha = lerp(_alpha, 1, 1 - power(0.92, 1 / global.fps_scale));
    y = lerp(y, _card_y, 1 - power(0.84, 1 / global.fps_scale));
    if (_alpha >= 0.995)
    {
        _alpha = 1;
        _fade_in = false;
    }
}
if (_is_highlighted)
{
    _color = make_color_rgb(_th_r, _th_g, _th_b);
}
else
{
    _color = 16777215;
}
