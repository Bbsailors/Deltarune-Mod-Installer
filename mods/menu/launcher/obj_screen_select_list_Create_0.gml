_parent = -4;
_input_enabled = false;
_input_buffer = round(2 * global.fps_scale);
_input_time = _input_buffer;
_slots = [];
_chapters = [];
_chapter_index = 0;
_fade_in = false;
_scroll = 185;
_target_scroll = 185;
_card_spacing = 210;
_card_first = 40;
_card_count = 7;
_head_alpha = 0;
for (var i = 0; i < _card_count; i++)
{
    var chapter = i + 1;
    var new_chapter = instance_create(_card_first + (i * _card_spacing), 88, obj_ui_chapter);
    new_chapter.init(id, chapter);
    new_chapter.enable();
    _slots[array_length(_slots)] = new_chapter;
    _chapters[array_length(_chapters)] = new_chapter;
}

init = function(arg0)
{
    _parent = arg0;
    _init = true;
};

fade_in = function()
{
    _fade_in = true;
    for (var i = 0; i < array_length(_slots); i++)
    {
        _slots[i].fade_in();
    }
};

update_scroll = function(arg0)
{
    _target_scroll = 185 - (_chapter_index * _card_spacing);
    if (arg0)
    {
        _scroll = _target_scroll;
    }
};

reset = function()
{
    for (var i = 0; i < array_length(_chapters); i++)
    {
        _chapters[i].reset();
    }
};

highlight_chapter = function(arg0)
{
    var idx = arg0 - 1;
    if (idx < 0)
    {
        idx = 0;
    }
    if (idx >= array_length(_chapters))
    {
        idx = array_length(_chapters) - 1;
    }
    _chapter_index = idx;
    highlight();
};

highlight = function()
{
    for (var i = 0; i < array_length(_chapters); i++)
    {
        var chapter = _chapters[i];
        chapter.reset();
        if (i == _chapter_index)
        {
            chapter.highlight();
        }
    }
    update_scroll(!_fade_in);
};

highlight_bottom = function()
{
    _chapter_index = array_length(_chapters) - 1;
    highlight();
};

enable_input = function()
{
    _input_enabled = true;
    _input_time = _input_buffer;
};

disable_input = function()
{
    _input_enabled = false;
};

disable_all = function()
{
    _input_enabled = false;
};

get_selected_chapter = function()
{
    if (_chapter_index < 0)
    {
        return 1;
    }
    if (_chapter_index >= array_length(_chapters))
    {
        return 1;
    }
    return _chapters[_chapter_index].get_chapter();
};

get_selected_has_data = function()
{
    if (_chapter_index < 0)
    {
        return false;
    }
    if (_chapter_index >= array_length(_chapters))
    {
        return false;
    }
    return _chapters[_chapter_index].get_has_data();
};

trigger_event = function(arg0, arg1)
{
    if (arg0 == "launch_game")
    {
        disable_all();
        _parent.trigger_event("launch_game", arg1);
    }
};
