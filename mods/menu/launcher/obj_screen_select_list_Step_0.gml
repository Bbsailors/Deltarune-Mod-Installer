global.menu_theme = _chapter_index + 1;
_head_alpha = lerp(_head_alpha, 1, 1 - power(0.95, 1 / global.fps_scale));
_scroll = lerp(_scroll, _target_scroll, 1 - power(0.84, 1 / global.fps_scale));
for (var i = 0; i < array_length(_slots); i++)
{
    var c = _slots[i];
    c.x = c._base_x + _scroll;
    c.depth = -10 + abs(i - _chapter_index);
}
if (!_input_enabled)
{
    exit;
}
_input_time--;
if (_input_time > 0)
{
    exit;
}
if (left_p())
{
    if (_chapter_index > 0)
    {
        _input_time = _input_buffer;
        audio_play_sound(snd_menumove, 50, 0);
        _chapter_index -= 1;
        highlight();
    }
}
else if (right_p())
{
    if (_chapter_index < (array_length(_chapters) - 1))
    {
        _input_time = _input_buffer;
        audio_play_sound(snd_menumove, 50, 0);
        _chapter_index += 1;
        highlight();
    }
}
else if (up_p())
{
    _input_time = _input_buffer;
    audio_play_sound(snd_menumove, 50, 0);
    _parent.trigger_event("scroll_list_up");
}
else if (down_p())
{
    _input_time = _input_buffer;
    audio_play_sound(snd_menumove, 50, 0);
    _parent.trigger_event("scroll_list_down");
}
else if (button1_p())
{
    _input_time = _input_buffer;
    if (get_selected_has_data())
    {
        audio_play_sound(snd_menumove, 50, 0);
        trigger_event("launch_game", get_selected_chapter());
    }
    else
    {
        audio_play_sound(snd_swing, 50, 0);
    }
}
