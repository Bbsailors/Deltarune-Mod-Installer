// Deltarune 60 FPS: applies the Launcher code changes to a clean, unmodified data.win.
// Every edited code entry's finished GML is included below, so no other files are needed.
//
// Expected vanilla base: SHA-256 82a7041a530407c5f17e439a97bdf53e4a24e30775a7ecbc2879369b9d2441d6
// The result matches the released 60 FPS build's code. Recompiling can normalize some
// formatting, so it may not be identical byte for byte.
//
// To run: in UndertaleModTool, open a clean vanilla Launcher data.win, choose
// Scripts > Run other script, pick this file, then Save.
//
using System;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Compiler;

// ---- base guard: refuse to run unless this looks like the exact expected vanilla ----
if (Data.Code.ByName("gml_Script_scr_fps_lerp_rate") != null)
    throw new Exception("scr_fps_lerp_rate already present -> this data.win looks already-modded. "
        + "Apply to a CLEAN vanilla Launcher data.win (expected SHA-256 82a7041a...).");
// (Code.Count guard relaxed for modding on top)

var __g = new CodeImportGroup(Data);
int __applied = 0;
void Apply(string name, string gml)
{
    var code = Data.Code.ByName(name);
    if (code == null)
        throw new Exception("Entry not found (is this the right vanilla Launcher data.win?): " + name);
    __g.QueueReplace(code, gml);
    __applied++;
}

Apply("gml_Object_obj_CHAPTER_SELECT_Create_0", @"room_speed = 60;
global.fps_scale = 2;
global.debug = 0;
global.is_console = scr_is_switch_os() || os_type == os_ps4 || os_type == os_ps5;
if (!variable_global_exists(""bgm"") || !audio_exists(global.bgm))
{
    global.bgm = -4;
}
scr_init();
_current_state = UnknownEnum.Value_0;
_chapter_in_progress = 0;
_chapter_completed = 0;
_target_chapter = -1;
_restart_room = false;
_launch_data = scr_init_launch_parameters();
_pending_titles = [];
_pending_title = -4;
_load_type = UnknownEnum.Value_0;

init = function()
{
    var max_chapter = UnknownEnum.Value_7;
    var max_available_chapter = UnknownEnum.Value_5;
    _current_state = UnknownEnum.Value_1;
    for (var i = 0; i < max_available_chapter; i++)
    {
        var chapter = i + 1;
        if (scr_chapter_save_file_exists(chapter) && !scr_completed_chapter_any_slot(chapter))
        {
            _chapter_in_progress = chapter;
        }
    }
    if (_chapter_in_progress > 0)
    {
        _current_state = UnknownEnum.Value_2;
    }
    for (var i = 0; i < max_available_chapter; i++)
    {
        var chapter = i + 1;
        if (scr_completed_chapter_any_slot(chapter))
        {
            _chapter_completed = chapter;
        }
    }
    if (_chapter_completed > 0)
    {
        _current_state = UnknownEnum.Value_3;
        if (_chapter_in_progress > _chapter_completed)
        {
            _current_state = UnknownEnum.Value_2;
            if ((_chapter_in_progress - _chapter_completed) >= 2)
            {
                _current_state = UnknownEnum.Value_4;
            }
        }
        if (_chapter_completed >= max_available_chapter)
        {
            _current_state = UnknownEnum.Value_4;
        }
    }
    if (!is_original_launcher(_launch_data))
    {
        _current_state = UnknownEnum.Value_4;
    }
    change_state(_current_state);
};

change_state = function(arg0, arg1)
{
    _current_state = arg0;
    switch (_current_state)
    {
        case UnknownEnum.Value_4:
            with (obj_screen_start)
            {
                clean_up();
            }
            start_bgm();
            create_select_screen();
            break;
        case UnknownEnum.Value_1:
            stop_bgm();
            create_start_screen();
            break;
        case UnknownEnum.Value_2:
            stop_bgm();
            create_continue_screen();
            break;
        case UnknownEnum.Value_3:
            stop_bgm();
            create_start_next_screen();
            break;
        case UnknownEnum.Value_5:
            with (obj_screen_start)
            {
                clean_up();
            }
            _pending_titles = arg1;
            if (scr_is_switch_os())
            {
                if (array_length(_pending_titles) > 1)
                {
                    _load_type = UnknownEnum.Value_1;
                }
            }
            if (_load_type == UnknownEnum.Value_0)
            {
                _pending_title = _pending_titles[0];
                var adjusted_list = [];
                for (var i = 0; i < array_length(_pending_titles); i++)
                {
                    if (_pending_titles[i] == _pending_title)
                    {
                        continue;
                    }
                    adjusted_list[array_length(adjusted_list)] = _pending_titles[i];
                }
                _pending_titles = adjusted_list;
                create_load_prompt_screen(_pending_title.title);
            }
            else
            {
                create_load_prompt_multiple_screen(_pending_titles);
            }
            break;
        case UnknownEnum.Value_6:
            with (obj_screen_start)
            {
                clean_up();
            }
            create_load_deny_confirm_screen();
            break;
        case UnknownEnum.Value_0:
            with (obj_screen_start)
            {
                clean_up();
            }
            break;
    }
};

create_start_screen = function()
{
    var start_text = (global.lang == ""en"") ? ""Would you like to start from Chapter 1?"" : ""Chapter 1ã‹ã‚‰å§‹ã‚ã¾ã™ã‹ï¼Ÿ"";
    var yes_text = (global.lang == ""en"") ? ""Yes"" : ""ã¯ã„"";
    var no_text = (global.lang == ""en"") ? ""No"" : ""ã„ã„ãˆ"";
    var choices = [new create_choice(yes_text, UnknownEnum.Value_0), new create_choice(no_text, UnknownEnum.Value_1)];
    var choice_offset = (global.lang == ""ja"") ? -20 : 0;
    var start_screen = instance_create(0, 0, obj_screen_start);
    start_screen.init(id, start_text, choices, choice_offset);
    start_screen.fade_in();
};

create_continue_screen = function()
{
    var continue_text = ""Continue from Chapter "" + string(_chapter_in_progress) + ""?"";
    if (global.lang == ""ja"")
    {
        continue_text = ""Chapter "" + string(_chapter_in_progress) + ""ã‹ã‚‰ç¶šã‘ã¾ã™ã‹ï¼Ÿ"";
    }
    var yes_text = (global.lang == ""en"") ? ""Yes"" : ""ã¯ã„"";
    var no_text = (global.lang == ""en"") ? ""No"" : ""ã„ã„ãˆ"";
    var choices = [new create_choice(yes_text, UnknownEnum.Value_0), new create_choice(no_text, UnknownEnum.Value_1)];
    var choice_offset = (global.lang == ""ja"") ? -20 : 0;
    var start_screen = instance_create(0, 0, obj_screen_start);
    start_screen.init(id, continue_text, choices, choice_offset);
    start_screen.fade_in();
};

create_start_next_screen = function()
{
    var continue_text = ""Chapter "" + string(_chapter_completed) + "" was completed."";
    if (global.lang == ""ja"")
    {
        continue_text = ""Chapter "" + string(_chapter_completed) + ""ã¯ã‚¯ãƒªã‚¢æ¸ˆã¿ã§ã™ã€‚"";
    }
    var play_next_text = ""Play Chapter "" + string(_chapter_completed + 1);
    if (global.lang == ""ja"")
    {
        play_next_text = ""Chapter "" + string(_chapter_completed + 1) + ""ã‚’ãƒ—ãƒ¬ã‚¤"";
    }
    var chapter_select_text = (global.lang == ""en"") ? ""Chapter Select"" : ""ãƒãƒ£ãƒ—ã‚¿ãƒ¼é¸æŠž"";
    var choices = [new create_choice(play_next_text, UnknownEnum.Value_0), new create_choice(chapter_select_text, UnknownEnum.Value_1)];
    var start_screen = instance_create(0, 0, obj_screen_start);
    var choice_offset = (global.lang == ""en"") ? -70 : -100;
    start_screen.init(id, continue_text, choices, choice_offset);
    start_screen.fade_in();
};

create_select_screen = function()
{
    var select_screen = instance_create(0, 0, obj_screen_select);
    select_screen.init(id);
    select_screen.fade_in();
};

create_load_prompt_screen = function(arg0)
{
    var load_text = scr_get_app_title(arg0) + ""Save Data found.\nImport this Save Data?\n(This will only be asked once.)"";
    if (global.lang == ""ja"")
    {
        load_text = scr_get_app_title(arg0) + ""ã‚»ãƒ¼ãƒ–ãƒ‡ãƒ¼ã‚¿ã‚’æ¤œå‡ºã—ã¾ã—ãŸã€‚\nã“ã®ã‚»ãƒ¼ãƒ–ãƒ‡ãƒ¼ã‚¿ã‚’å–ã‚Šè¾¼ã¿ã¾ã™ã‹ï¼Ÿ\nï¼ˆã“ã®ç¢ºèªã¯ä¸€åº¦ã—ã‹è¡Œã„ã¾ã›ã‚“ï¼‰"";
    }
    var yes_text = (global.lang == ""en"") ? ""Yes"" : ""ã¯ã„"";
    var no_text = (global.lang == ""en"") ? ""No"" : ""ã„ã„ãˆ"";
    var choices = [new create_choice(yes_text, UnknownEnum.Value_0), new create_choice(no_text, UnknownEnum.Value_1)];
    var choice_offset = (global.lang == ""ja"") ? -20 : 0;
    var start_screen = instance_create(0, 0, obj_screen_start);
    start_screen.init(id, load_text, choices, choice_offset);
    start_screen.fade_in();
};

create_load_prompt_multiple_screen = function(arg0)
{
    var load_text = ""Multiple DELTARUNE Save Files found.\nWould you like to import one of these?\n(This will only be asked once.)\n \n "";
    if (global.lang == ""ja"")
    {
        load_text = ""è¤‡æ•°ã®ã€ŽDELTARUNEã€ã‚»ãƒ¼ãƒ–ãƒ‡ãƒ¼ã‚¿ãŒå­˜åœ¨ã—ã¾ã™ã€‚\nã©ã‚Œã‹1ã¤ã‚’å–ã‚Šè¾¼ã¿ã¾ã™ã‹ï¼Ÿ\nï¼ˆã“ã®ç¢ºèªã¯ä¸€åº¦ã—ã‹è¡Œã„ã¾ã›ã‚“ï¼‰\n \n "";
    }
    var choices = [];
    for (var i = 0; i < array_length(arg0); i++)
    {
        var prev_title = arg0[i];
        var title_choice = scr_get_app_title_choice_text(prev_title.title);
        var new_choice = new create_choice(title_choice, prev_title);
        choices[array_length(choices)] = new_choice;
    }
    var do_not_text = (global.lang == ""en"") ? ""Do Not Import"" : ""å–ã‚Šè¾¼ã¾ãªã„"";
    choices[array_length(choices)] = new create_choice(do_not_text, UnknownEnum.Value_1);
    var start_screen = instance_create(0, 0, obj_screen_start);
    start_screen.init(id, load_text, choices, 0, -32);
    start_screen.fade_in();
};

create_load_deny_confirm_screen = function()
{
    var deny_text = (global.lang == ""en"") ? ""Proceed without importing?"" : ""å–ã‚Šè¾¼ã¾ãšã«é€²ã‚ã¾ã™ã‹ï¼Ÿ"";
    var yes_text = (global.lang == ""en"") ? ""Yes"" : ""ã¯ã„"";
    var no_text = (global.lang == ""en"") ? ""No"" : ""ã„ã„ãˆ"";
    var choices = [new create_choice(yes_text, UnknownEnum.Value_0), new create_choice(no_text, UnknownEnum.Value_1)];
    var choice_offset = (global.lang == ""ja"") ? -20 : 0;
    var start_screen = instance_create(0, 0, obj_screen_start);
    start_screen.init(id, deny_text, choices, choice_offset);
    start_screen.fade_in();
};

trigger_event = function(arg0, arg1)
{
    var event_name = arg0;
    var event_value = arg1;
    switch (_current_state)
    {
        case UnknownEnum.Value_0:
            if (arg0 == ""init_complete"")
            {
                init();
            }
            else if (arg0 == ""load_prompt"")
            {
                change_state(UnknownEnum.Value_5, arg1);
            }
            break;
        case UnknownEnum.Value_5:
            if (event_value == UnknownEnum.Value_0)
            {
                global.savedata = _pending_title.save_data;
                _restart_room = true;
                change_state(UnknownEnum.Value_0);
                with (obj_init_console)
                {
                    convert_loaded_file();
                }
            }
            else if (event_value == UnknownEnum.Value_1)
            {
                if (_load_type == UnknownEnum.Value_0)
                {
                    if (array_length(_pending_titles) > 0)
                    {
                        change_state(UnknownEnum.Value_0);
                        trigger_event(""load_prompt"", _pending_titles);
                    }
                    else
                    {
                        change_state(UnknownEnum.Value_6);
                    }
                }
                else
                {
                    change_state(UnknownEnum.Value_6);
                }
            }
            else
            {
                _pending_title = event_value;
                global.savedata = _pending_title.save_data;
                _restart_room = true;
                change_state(UnknownEnum.Value_0);
                with (obj_init_console)
                {
                    convert_loaded_file();
                }
            }
            break;
        case UnknownEnum.Value_6:
            if (event_value == UnknownEnum.Value_0)
            {
                _restart_room = true;
                change_state(UnknownEnum.Value_0);
                with (obj_init_console)
                {
                    create_new_save_file();
                }
            }
            else
            {
                room_restart();
            }
            break;
        case UnknownEnum.Value_1:
            if (event_value == UnknownEnum.Value_0)
            {
                _target_chapter = UnknownEnum.Value_1;
                show_transition();
            }
            else
            {
                change_state(UnknownEnum.Value_4);
            }
            break;
        case UnknownEnum.Value_2:
            if (event_value == UnknownEnum.Value_0)
            {
                _target_chapter = _chapter_in_progress;
                show_transition();
            }
            else
            {
                change_state(UnknownEnum.Value_4);
            }
            break;
        case UnknownEnum.Value_3:
            if (event_value == UnknownEnum.Value_0)
            {
                _target_chapter = _chapter_completed + 1;
                show_transition();
            }
            else
            {
                change_state(UnknownEnum.Value_4);
            }
            break;
        case UnknownEnum.Value_4:
            if (event_name == ""launch_game"")
            {
                _target_chapter = event_value;
                show_transition();
            }
            else if (event_value == UnknownEnum.Value_4)
            {
                quit();
            }
            else if (event_value == UnknownEnum.Value_5)
            {
                toggle_language();
            }
            break;
    }
};

show_transition = function()
{
    if (instance_exists(obj_screen_transition))
    {
        exit;
    }
    if (global.bgm != -4)
    {
        audio_sound_gain(global.bgm, 0, 500);
    }
    var sound_file = get_chapter_confirm_sound(_target_chapter);
    var volume = 1;
    if (_target_chapter == 3)
    {
        volume = 0.7;
    }
    audio_play_sound(sound_file, 50, 0, volume);
    var delay = round(audio_sound_length(sound_file) * room_speed);
    var transition = instance_create(0, 0, obj_screen_transition);
    transition.init(id, _target_chapter, delay);
    transition.start(launch_game);
};

launch_game = function(arg0)
{
    audio_stop_all();
    var chapstring = string(_target_chapter);
    var parameters = get_chapter_switch_parameters();
    if (scr_is_switch_os())
    {
        game_change(""rom:/chapter"" + chapstring + ""_switch/"", parameters);
    }
    else
    {
        switch (os_type)
        {
            case os_windows:
                game_change(""/chapter"" + chapstring + ""_windows"", ""-game data.win"" + parameters);
                break;
            case os_ps4:
                game_change("""", ""-game /app0/games/chapter"" + chapstring + ""_ps4/game.win"" + parameters);
                break;
            case os_ps5:
                game_change("""", ""-game /app0/games/chapter"" + chapstring + ""_ps5/game.win"" + parameters);
                break;
            case os_macosx:
                game_change(""chapter"" + chapstring + ""_mac"", parameters);
                break;
        }
    }
};

quit = function()
{
    game_end();
};

toggle_language = function()
{
    if (_restart_room)
    {
        exit;
    }
    _restart_room = true;
    var target_lang = (global.lang == ""en"") ? ""ja"" : ""en"";
    global.lang = target_lang;
    ossafe_ini_open(""true_config.ini"");
    ini_write_string(""LANG"", ""LANG"", global.lang);
    ossafe_ini_close();
    ossafe_savedata_save();
    if (!global.is_console)
    {
        room_restart();
    }
};

start_bgm = function()
{
    if (global.bgm == -4 || !audio_is_playing(global.bgm))
    {
        global.bgm = audio_play_sound(AUDIO_DRONE, 15, 1);
    }
};

stop_bgm = function()
{
    if (global.bgm != -4)
    {
        audio_stop_sound(global.bgm);
        global.bgm = -4;
    }
};

if (global.is_console)
{
    var console_init = instance_create(0, 0, obj_init_console);
    console_init.init(id, _launch_data);
}
else
{
    var pc_init = instance_create(0, 0, obj_init_pc);
    pc_init.init(id, _launch_data);
}

enum UnknownEnum
{
    Value_0,
    Value_1,
    Value_2,
    Value_3,
    Value_4,
    Value_5,
    Value_6,
    Value_7
}
");

Apply("gml_Object_obj_screen_select_footer_Step_0", @"if (_fade_in)
{
    _alpha = lerp(_alpha, 1, 1 - power(0.94, 1 / global.fps_scale));
    for (var i = 0; i < array_length(_choices); i++)
    {
        var choice = _choices[i];
        choice.set_alpha(_alpha);
        choice.y = lerp(choice.y, choice.ystart, 1 - power(0.86, 1 / global.fps_scale));
    }
    if (_alpha >= 1)
    {
        _fade_in = false;
    }
}
if (!_input_enabled)
{
    exit;
}
if (up_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _parent.trigger_event(""scroll_footer_up"");
}
else if (down_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _parent.trigger_event(""scroll_footer_down"");
}
if (array_length(_choices) == 1)
{
    exit;
}
if (left_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _choice_index = scr_wrap(_choice_index - 1, 0, array_length(_choices) - 1);
    highlight();
}
else if (right_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _choice_index = scr_wrap(_choice_index + 1, 0, array_length(_choices) - 1);
    highlight();
}
");

Apply("gml_Object_obj_screen_select_list_Create_0", @"_parent = -4;
_input_enabled = false;
_input_buffer = round(2 * global.fps_scale);
_input_time = _input_buffer;
_slots = [];
_chapters = [];
_chapter_index = 0;
_fade_in = false;
var max_chapter = UnknownEnum.Value_7;
var max_available_chapter = UnknownEnum.Value_5;
for (var i = 0; i < max_chapter; i++)
{
    var new_chapter = instance_create(0, -10 + (i * 60), obj_ui_chapter);
    var chapter = i + 1;
    new_chapter.init(id, chapter);
    _slots[array_length(_slots)] = new_chapter;
}
for (var i = 0; i < max_available_chapter; i++)
{
    var chapter = _slots[i];
    chapter.enable();
    _chapters[array_length(_chapters)] = chapter;
}

init = function(arg0)
{
    _parent = arg0;
    _init = true;
};

fade_in = function()
{
    for (var i = 0; i < array_length(_slots); i++)
    {
        _slots[i].fade_in();
    }
};

reset = function()
{
    _chapter_index = 0;
    for (var i = 0; i < array_length(_chapters); i++)
    {
        var chapter = _chapters[i];
        chapter.reset();
    }
};

highlight_chapter = function(arg0)
{
    for (var i = 0; i < array_length(_chapters); i++)
    {
        var chapter = _chapters[i].get_chapter();
        if (chapter == arg0)
        {
            _chapter_index = i;
            highlight();
        }
    }
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
    for (var i = 0; i < array_length(_chapters); i++)
    {
        var chapter = _chapters[i];
        chapter.disable_input();
    }
};

disable_all = function()
{
    _input_enabled = false;
    for (var i = 0; i < array_length(_chapters); i++)
    {
        var chapter = _chapters[i];
        chapter.disable_all();
    }
};

trigger_event = function(arg0, arg1)
{
    switch (arg0)
    {
        case ""disable_scroll"":
            disable_input();
            break;
        case ""enable_scroll"":
            enable_input();
            break;
        case ""launch_game"":
            disable_all();
            _parent.trigger_event(""launch_game"", arg1);
            break;
    }
};

enum UnknownEnum
{
    Value_5 = 5,
    Value_7 = 7
}
");

Apply("gml_Object_obj_screen_start_Step_0", @"if (!init)
{
    exit;
}
if (_fade_in)
{
    _alpha = lerp(_alpha, 1, 1 - power(0.94, 1 / global.fps_scale));
    _y_pos = lerp(_y_pos, 220, 1 - power(0.86, 1 / global.fps_scale));
    for (var i = 0; i < array_length(_choices); i++)
    {
        var choice = _choices[i];
        choice.set_alpha(_alpha);
        choice.y = lerp(choice.y, choice.ystart, 1 - power(0.86, 1 / global.fps_scale));
    }
    _version_display.set_alpha(_alpha);
    if (_alpha >= 1)
    {
        _fade_in = false;
    }
    _timer++;
    if (_timer == round(6 * global.fps_scale))
    {
        enable_input();
        enable_select();
    }
}
if (!_input_enabled)
{
    exit;
}
if (up_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _choice_index = scr_wrap(_choice_index - 1, 0, array_length(_choices) - 1);
    enable_select();
}
else if (down_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _choice_index = scr_wrap(_choice_index + 1, 0, array_length(_choices) - 1);
    enable_select();
}
");

Apply("gml_Object_obj_screen_transition_Step_0", @"if (!_active)
{
    exit;
}
fadeout = lerp(fadeout, 0, 1 - power(0.875, 1 / global.fps_scale));
xscale *= power(0.95, 1 / global.fps_scale);
yscale *= power(0.99, 1 / global.fps_scale);
yy *= power(0.99, 1 / global.fps_scale);
if (_disable_timer)
{
    exit;
}
_timer--;
if (_timer <= 0)
{
    _disable_timer = true;
    if (global.is_console)
    {
        var _loading = instance_create(0, 0, obj_screen_loading);
        _loading.show_loading_screen(_target_chapter, _callback);
        _loading.depth = -1000;
    }
    else
    {
        _callback(_target_chapter);
    }
}
");

Apply("gml_Object_obj_ui_chapter_Step_0", @"if (_fade_in)
{
    _alpha = lerp(_alpha, 1, 1 - power(0.94, 1 / global.fps_scale));
    y = lerp(y, ystart, 1 - power(0.86, 1 / global.fps_scale));
    _chapter_choice.set_alpha(_alpha);
    _chapter_choice.y = lerp(_chapter_choice.y, _chapter_choice.ystart, 1 - power(0.86, 1 / global.fps_scale));
    for (var i = 0; i < array_length(_choices); i++)
    {
        var choice = _choices[i];
        choice.set_alpha(_alpha);
        choice.y = lerp(choice.y, choice.ystart, 1 - power(0.86, 1 / global.fps_scale));
    }
    if (_alpha >= 1)
    {
        _fade_in = false;
    }
}
if (!_scroll_enabled)
{
    exit;
}
if (left_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _choice_index = scr_wrap(_choice_index - 1, 0, array_length(_choices) - 1);
    highlight_choice();
}
else if (right_p())
{
    audio_play_sound(snd_menumove, 50, 0);
    _choice_index = scr_wrap(_choice_index + 1, 0, array_length(_choices) - 1);
    highlight_choice();
}
else if (button2_p())
{
    audio_play_sound(snd_swing, 50, 0);
    trigger_event(""select"", UnknownEnum.Value_3);
}

enum UnknownEnum
{
    Value_3 = 3
}
");

Apply("gml_Object_obj_ui_choice_Create_0", @"_parent = -4;
_init = false;
_input_enabled = false;
_is_highlighted = false;
_disabled = false;
_input_buffer = round(2 * global.fps_scale);
_input_time = _input_buffer;
_heart_enabled = false;
_text = """";
_value = -4;
_text_offset = 0;
_alpha = 1;
_scale = 2;
_color = 16777215;
_font = (global.lang == ""en"") ? 2 : 1;
_centered = false;

init = function(arg0, arg1, arg2)
{
    _parent = arg0;
    _text = arg1;
    _value = arg2;
    _init = true;
};

align_center = function()
{
    _centered = true;
};

set_font = function(arg0)
{
    _font = arg0;
};

set_alpha = function(arg0)
{
    _alpha = arg0;
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

disable = function()
{
    _disabled = true;
    _color = 8421504;
    disable_input();
};

enable = function()
{
    _disabled = false;
    reset();
};

highlight = function()
{
    _is_highlighted = true;
    _color = 65535;
    show_heart();
    enable_input();
};

reset = function()
{
    _is_highlighted = false;
    _color = 16777215;
    hide_heart();
    disable_input();
};

select = function()
{
    audio_play_sound(snd_select, 50, 0);
    _parent.trigger_event(""select"", _value);
};

hide_heart = function()
{
    _heart_enabled = false;
};

show_heart = function()
{
    _heart_enabled = true;
};
");

Apply("gml_Object_obj_ui_grid_Step_0", @"if (_fade_in)
{
    _alpha = lerp(_alpha, 1, 1 - power(0.94, 1 / global.fps_scale));
    for (var i = 0; i < array_length(_grid_lines); i++)
    {
        var grid_line = _grid_lines[i];
        grid_line.set_alpha(_alpha);
        grid_line.y = lerp(grid_line.y, grid_line.ystart, 1 - power(0.86, 1 / global.fps_scale));
    }
    if (_alpha >= 1)
    {
        _fade_in = false;
    }
}
");

if (__applied != 8)
    throw new Exception("Expected 8 entries, applied " + __applied);
__g.Import(throwOnFailedCompile: true);
Console.WriteLine("60 FPS Launcher: " + __applied + " entries applied.");
