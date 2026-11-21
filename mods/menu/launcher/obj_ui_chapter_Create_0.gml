_chapter = 0;
_parent = -4;
_chapter_title = "";
_title_enabled = true;
_choices = [];
_choice_index = 0;
_scroll_enabled = false;
_input_enabled = false;
_enabled = false;
_is_highlighted = false;
_alpha = 0;
_color = 8421504;
_font = (global.lang == "en") ? 2 : 1;
_icon_index = 0;
_completed_files = [];
_has_data = false;
_fade_in = false;
_card_w = 190;
_card_h = 300;
_card_y = 88;
_base_x = 0;
ystart = 0;
_th_r = 255;
_th_g = 214;
_th_b = 150;

init = function(arg0, arg1)
{
    _parent = arg0;
    _chapter = arg1;
    _chapter_title = get_chapter_title(_chapter);
    _base_x = 40 + ((_chapter - 1) * 210);
    x = _base_x;
    y = _card_y;
    ystart = y;
    switch (_chapter)
    {
        case 1:
            _th_r = 255;
            _th_g = 214;
            _th_b = 150;
            break;
        case 2:
            _th_r = 96;
            _th_g = 220;
            _th_b = 255;
            break;
        case 3:
            _th_r = 198;
            _th_g = 150;
            _th_b = 255;
            break;
        case 4:
            _th_r = 120;
            _th_g = 235;
            _th_b = 200;
            break;
        case 5:
            _th_r = 255;
            _th_g = 140;
            _th_b = 100;
            break;
        default:
            _th_r = 150;
            _th_g = 158;
            _th_b = 185;
            break;
    }
    _init = true;
};

get_chapter = function()
{
    return _chapter;
};

get_has_data = function()
{
    return _has_data;
};

fade_in = function()
{
    _fade_in = true;
    _alpha = 0;
    y = _card_y + 18;
};

enable = function()
{
    _enabled = true;
    _icon_index = get_chapter_icon_index(_chapter);
    for (var i = 0; i < 3; i++)
    {
        _completed_files[i] = scr_completed_chapter_in_slot(_chapter, i);
    }
    for (var i = 0; i < 3; i++)
    {
        var completed = _completed_files[i] == 1;
        var current_exists = scr_chapter_save_file_exists_in_slot(_chapter, i);
        if (completed && !current_exists)
        {
            _completed_files[i] = 2;
        }
    }
    _has_data = ossafe_file_exists("chapter" + string(_chapter) + "_windows/data.win");
};

reset = function()
{
    _is_highlighted = false;
    _color = 16777215;
    _input_enabled = false;
};

highlight = function()
{
    _is_highlighted = true;
    _input_enabled = true;
};

enable_input = function()
{
    _input_enabled = true;
};

disable_input = function()
{
    _input_enabled = false;
};

disable_all = function()
{
    _input_enabled = false;
};

hide_title = function()
{
    _title_enabled = false;
};

show_title = function()
{
    _title_enabled = true;
};

trigger_event = function(arg0, arg1)
{
    if (arg1 == 2)
    {
        _parent.trigger_event("launch_game", _chapter);
    }
};
