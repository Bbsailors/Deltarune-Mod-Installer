if (!input_enabled)
{
    exit;
}
if (MENU_NO == 1 || MENU_NO == 4 || MENU_NO == 6 || MENU_NO == 7)
{
    if (left_p())
    {
        if (MENUCOORD[MENU_NO] == 1)
        {
            MENUCOORD[MENU_NO] = 0;
            MOVENOISE = 1;
        }
    }
    if (right_p())
    {
        if (MENUCOORD[MENU_NO] == 0)
        {
            MENUCOORD[MENU_NO] = 1;
            MOVENOISE = 1;
        }
    }
    if (button1_p() && ONEBUFFER < 0)
    {
        ONEBUFFER = 2;
        TWOBUFFER = 2;
        SELNOISE = 1;
        if (MENUCOORD[MENU_NO] == 0)
        {
            if (MENU_NO == 1)
            {
                if (FILE[MENUCOORD[0]] == 1)
                {
                    global.filechoice = MENUCOORD[0];
                    scr_windowcaption(scr_84_get_lang_string("DEVICE_MENU_slash_Step_0_gml_35_0"));
                    snd_free_all();
                    obj_loadscreen.loaded = true;
                    input_enabled = false;
                    if (ossafe_file_exists("keyconfig_" + string(global.filechoice) + ".ini"))
                    {
                        ossafe_ini_open("keyconfig_" + string(global.filechoice) + ".ini");
                        for (var i = 0; i < 10; i += 1)
                        {
                            readval = ini_read_real("KEYBOARD_CONTROLS", string(i), -1);
                            if (readval != -1)
                            {
                                global.input_k[i] = readval;
                            }
                        }
                        for (var i = 0; i < 10; i += 1)
                        {
                            readval = ini_read_real("GAMEPAD_CONTROLS", string(i), -1);
                            if (readval != -1)
                            {
                                global.input_g[i] = readval;
                            }
                        }
                        readval = ini_read_real("SHOULDERLB_REASSIGN", "SHOULDERLB_REASSIGN", obj_gamecontroller.gamepad_shoulderlb_reassign);
                        if (readval != -1)
                        {
                            obj_gamecontroller.gamepad_shoulderlb_reassign = readval;
                        }
                        global.button0 = global.input_g[4];
                        global.button1 = global.input_g[5];
                        global.button2 = global.input_g[6];
                        global.screen_border_id = ini_read_string("BORDER", "TYPE", "Dynamic");
                        var _disable_border = global.screen_border_id == "None" || global.screen_border_id == "ãªã—";
                        scr_enable_screen_border(!_disable_border);
                        ossafe_ini_close();
                        ossafe_savedata_save();
                    }
                    else if (ossafe_file_exists("config_" + string(global.filechoice) + ".ini"))
                    {
                        ossafe_ini_open("config_" + string(global.filechoice) + ".ini");
                        for (var i = 0; i < 10; i += 1)
                        {
                            readval = ini_read_real("KEYBOARD_CONTROLS", string(i), -1);
                            if (readval != -1)
                            {
                                global.input_k[i] = readval;
                            }
                        }
                        for (var i = 0; i < 10; i += 1)
                        {
                            readval = ini_read_real("GAMEPAD_CONTROLS", string(i), -1);
                            if (readval != -1)
                            {
                                global.input_g[i] = readval;
                            }
                        }
                        readval = ini_read_real("SHOULDERLB_REASSIGN", "SHOULDERLB_REASSIGN", obj_gamecontroller.gamepad_shoulderlb_reassign);
                        if (readval != -1)
                        {
                            obj_gamecontroller.gamepad_shoulderlb_reassign = readval;
                        }
                        global.input_g[0] = gp_padd;
                        global.input_g[1] = gp_padr;
                        global.input_g[2] = gp_padu;
                        global.input_g[3] = gp_padl;
                        global.input_g[4] = global.button0;
                        global.input_g[5] = global.button1;
                        global.input_g[6] = global.button2;
                        global.input_g[7] = 999;
                        global.input_g[8] = 999;
                        global.input_g[9] = 999;
                        global.button0 = global.input_g[4];
                        global.button1 = global.input_g[5];
                        global.button2 = global.input_g[6];
                        global.screen_border_id = ini_read_string("BORDER", "TYPE", "Dynamic");
                        var _disable_border = global.screen_border_id == "None" || global.screen_border_id == "ãªã—";
                        scr_enable_screen_border(!_disable_border);
                        ossafe_ini_close();
                        ossafe_savedata_save();
                        if (!global.is_console)
                        {
                            ossafe_ini_open("keyconfig_" + string(global.filechoice) + ".ini");
                            for (var i = 0; i < 10; i++)
                            {
                                ini_write_real("KEYBOARD_CONTROLS", string(i), global.input_k[i]);
                            }
                            for (var i = 0; i < 10; i++)
                            {
                                ini_write_real("GAMEPAD_CONTROLS", string(i), global.input_g[i]);
                            }
                            ini_write_real("SHOULDERLB_REASSIGN", "SHOULDERLB_REASSIGN", obj_gamecontroller.gamepad_shoulderlb_reassign);
                            ossafe_ini_close();
                        }
                    }
                    if (os_type == os_ps5)
                    {
                        with (obj_event_manager)
                        {
                            trigger_event(UnknownEnum.Value_2, UnknownEnum.Value_0);
                        }
                    }
                }
                if (FILE[MENUCOORD[0]] == 0)
                {
                    if (os_type == os_ps5)
                    {
                        with (obj_event_manager)
                        {
                            trigger_event(UnknownEnum.Value_2, UnknownEnum.Value_0);
                        }
                    }
                    global.filechoice = MENUCOORD[0];
                    snd_free_all();
                    room_goto(PLACE_CONTACT);
                }
            }
            if (MENU_NO == 4)
            {
                if (TYPE == 0)
                {
                    TEMPCOMMENT = "YANSIMA TAMAMLANDI.";
                    if (NAME[0] == NAME[1] && NAME[1] == NAME[2])
                    {
                        if (TIME[0] == TIME[1] && TIME[1] == TIME[2])
                        {
                            if (PLACE[0] == PLACE[1] && PLACE[1] == PLACE[2])
                            {
                                TEMPCOMMENT = "NE ILGINC BIR DAVRANIS.";
                            }
                        }
                    }
                }
                event_user(5);
                if (TYPE == 0)
                {
                    if (NAME[0] == NAME[1] && NAME[1] == NAME[2])
                    {
                        if (TIME[0] == TIME[1] && TIME[1] == TIME[2])
                        {
                            if (PLACE[0] == PLACE[1] && PLACE[1] == PLACE[2] && TEMPCOMMENT != "NE ILGINC BIR DAVRANIS.")
                            {
                                TEMPCOMMENT = "HAZIRLIKLAR TAMAMLANDI.";
                            }
                        }
                    }
                }
                if (TYPE == 1)
                {
                    TEMPCOMMENT = "Kopyalama tamamlandi.";
                }
                MESSAGETIMER = 90;
                SELNOISE = 0;
                DEATHNOISE = 1;
                MENU_NO = 0;
            }
            if (MENU_NO == 7)
            {
                FILE[MENUCOORD[5]] = 0;
                NAME[MENUCOORD[5]] = "[BOS]";
                TIME[MENUCOORD[5]] = 0;
                PLACE[MENUCOORD[5]] = "------------";
                LEVEL[MENUCOORD[5]] = 0;
                TIME_STRING[MENUCOORD[5]] = "--:--";
                ossafe_file_delete("filech1_" + string(MENUCOORD[5]));
                iniwrite = ossafe_ini_open("dr.ini");
                ini_write_string("G" + string(MENUCOORD[5]), "Name", "[BOS]");
                ini_write_real("G" + string(MENUCOORD[5]), "Level", 0);
                ini_write_real("G" + string(MENUCOORD[5]), "Love", 0);
                ini_write_real("G" + string(MENUCOORD[5]), "Time", 0);
                ini_write_real("G" + string(MENUCOORD[5]), "Room", 0);
                ini_write_real("G" + string(MENUCOORD[5]), "Date", 0);
                ini_write_real("G" + string(MENUCOORD[5]), "UraBoss", 0);
                ini_write_string("G" + string(MENUCOORD[5]), "Version", "0");
                ossafe_ini_close();
                ossafe_savedata_save();
                if (ossafe_file_exists("keyconfig_" + string(MENUCOORD[5]) + ".ini"))
                {
                    ossafe_file_delete("keyconfig_" + string(MENUCOORD[5]) + ".ini");
                }
                TEMPCOMMENT = "SANKI HIC VAR OLMAMIS GIBI.";
                if (TYPE == 1)
                {
                    TEMPCOMMENT = "Silme tamamlandi.";
                }
                MESSAGETIMER = 90;
                SELNOISE = 0;
                DEATHNOISE = 1;
                MENU_NO = 0;
                with (obj_event_manager)
                {
                    trigger_event(UnknownEnum.Value_0, UnknownEnum.Value_29);
                }
            }
            if (MENU_NO == 6)
            {
                THREAT += 1;
                MENU_NO = 7;
                MENUCOORD[7] = 0;
            }
        }
        if (MENUCOORD[MENU_NO] == 1)
        {
            if (MENU_NO == 4 && TYPE == 0)
            {
                TEMPCOMMENT = "ESKI SEKLINI KORUDU.";
                MESSAGETIMER = 90;
            }
            if (MENU_NO == 6 || MENU_NO == 7)
            {
                if (TYPE == 0)
                {
                    TEMPCOMMENT = "BAGISLANDI.";
                    if (THREAT >= 10)
                    {
                        TEMPCOMMENT = "COK ILGINC.";
                        THREAT = 0;
                    }
                    MESSAGETIMER = 90;
                }
            }
            MENU_NO = 0;
        }
    }
    if (button2_p() && TWOBUFFER < 0)
    {
        ONEBUFFER = 1;
        TWOBUFFER = 1;
        BACKNOISE = 1;
        if (MENU_NO == 1)
        {
            MENU_NO = 0;
        }
        if (MENU_NO == 4)
        {
            MENU_NO = 2;
        }
        if (MENU_NO == 6)
        {
            MENU_NO = 5;
        }
        if (MENU_NO == 7)
        {
            MENU_NO = 5;
        }
    }
}
if (MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5)
{
    if (MENUCOORD[MENU_NO] <= 2)
    {
        _last_slot = MENUCOORD[MENU_NO];
    }
    if (right_p())
    {
        if (MENUCOORD[MENU_NO] < 2)
        {
            MENUCOORD[MENU_NO] += 1;
            _last_slot = MENUCOORD[MENU_NO];
            MOVENOISE = 1;
        }
        else if (MENUCOORD[MENU_NO] == 2)
        {
            MENUCOORD[MENU_NO] = 3;
            MOVENOISE = 1;
        }
    }
    if (left_p())
    {
        if (MENUCOORD[MENU_NO] > 0 && MENUCOORD[MENU_NO] <= 2)
        {
            MENUCOORD[MENU_NO] -= 1;
            _last_slot = MENUCOORD[MENU_NO];
            MOVENOISE = 1;
        }
        else if (MENUCOORD[MENU_NO] == 3)
        {
            MENUCOORD[MENU_NO] = 2;
            _last_slot = 2;
            MOVENOISE = 1;
        }
    }
    if (down_p())
    {
        if (MENUCOORD[MENU_NO] < 3)
        {
            _last_slot = MENUCOORD[MENU_NO];
            MENUCOORD[MENU_NO] = 3;
            MOVENOISE = 1;
        }
    }
    if (up_p())
    {
        if (MENUCOORD[MENU_NO] == 3)
        {
            MENUCOORD[MENU_NO] = clamp(_last_slot, 0, 2);
            MOVENOISE = 1;
        }
        else if (MENUCOORD[MENU_NO] > 0)
        {
            MENUCOORD[MENU_NO] -= 1;
            _last_slot = MENUCOORD[MENU_NO];
            MOVENOISE = 1;
        }
    }
    if (button1_p() && ONEBUFFER < 0)
    {
        if (MENUCOORD[MENU_NO] < 3)
        {
            if (MENU_NO == 3)
            {
                if (MENUCOORD[2] != MENUCOORD[3])
                {
                    if (FILE[MENUCOORD[MENU_NO]] == 1)
                    {
                        TWOBUFFER = 2;
                        ONEBUFFER = 2;
                        SELNOISE = 1;
                        MENUCOORD[4] = 0;
                        MENU_NO = 4;
                    }
                    else
                    {
                        TEMPCOMMENT = "BOLUNME TAMAMLANDI.";
                        MESSAGETIMER = 90;
                        if (TYPE == 1)
                        {
                            TEMPCOMMENT = "Kopyalama tamamlandi.";
                        }
                        DEATHNOISE = 1;
                        MENU_NO = 0;
                        ONEBUFFER = 2;
                        TWOBUFFER = 2;
                        event_user(5);
                    }
                }
                else
                {
                    TEMPCOMMENT = "KENDI YANSIMASINA KOPYALANAMAZ.";
                    if (TYPE == 1)
                    {
                        TEMPCOMMENT = "Buraya kopyalanamaz.";
                    }
                    MESSAGETIMER = 90;
                    TWOBUFFER = 2;
                    ONEBUFFER = 2;
                    BACKNOISE = 1;
                }
            }
            if (MENU_NO == 2)
            {
                if (FILE[MENUCOORD[MENU_NO]] == 1)
                {
                    TWOBUFFER = 2;
                    ONEBUFFER = 2;
                    SELNOISE = 1;
                    MENUCOORD[3] = 0;
                    MENU_NO = 3;
                }
                else
                {
                    TEMPCOMMENT = "BOS DOSYA KOPYALANAMAZ.";
                    if (FILE[0] == 0 && FILE[1] == 0 && FILE[2] == 0)
                    {
                        TEMPCOMMENT = "KOPYALANACAK HICBIR SEY YOK.";
                    }
                    if (TYPE == 1)
                    {
                        TEMPCOMMENT = "Kopyalanamaz.";
                    }
                    MESSAGETIMER = 90;
                    BACKNOISE = 1;
                    TWOBUFFER = 2;
                    ONEBUFFER = 2;
                }
            }
            if (MENU_NO == 5)
            {
                if (FILE[MENUCOORD[MENU_NO]] == 1)
                {
                    TWOBUFFER = 2;
                    ONEBUFFER = 2;
                    SELNOISE = 1;
                    MENUCOORD[6] = 0;
                    MENU_NO = 6;
                }
                else
                {
                    TEMPCOMMENT = "ZATEN YOK OLMUSTU.";
                    if (FILE[0] == 0 && FILE[1] == 0 && FILE[2] == 0)
                    {
                        TEMPCOMMENT = "SILINECEK HICBIR SEY YOK.";
                    }
                    if (TYPE == 1)
                    {
                        TEMPCOMMENT = "Silinecek bir sey yok.";
                    }
                    MESSAGETIMER = 90;
                    TWOBUFFER = 2;
                    ONEBUFFER = 2;
                    BACKNOISE = 1;
                }
            }
        }
        if (MENUCOORD[MENU_NO] == 3)
        {
            TWOBUFFER = 2;
            ONEBUFFER = 2;
            SELNOISE = 1;
            MENU_NO = 0;
        }
    }
    if (button2_p() && TWOBUFFER < 0)
    {
        TWOBUFFER = 2;
        ONEBUFFER = 2;
        BACKNOISE = 1;
        if (MENU_NO == 2 || MENU_NO == 5)
        {
            MENU_NO = 0;
        }
        if (MENU_NO == 3)
        {
            MENU_NO = 2;
        }
    }
}
if (MENU_NO == 0)
{
    var memloc = MENUCOORD[0];
    var max_btn = global.is_console ? 6 : 7;
    if (MENUCOORD[0] <= 2)
    {
        _last_slot = MENUCOORD[0];
        if (right_p())
        {
            if (MENUCOORD[0] < 2)
            {
                MENUCOORD[0] += 1;
                _last_slot = MENUCOORD[0];
                MOVENOISE = 1;
            }
            else
            {
                _last_btn = clamp(_last_btn, 0, max_btn - 3);
                MENUCOORD[0] = 3 + _last_btn;
                MOVENOISE = 1;
            }
        }
        else if (left_p())
        {
            if (MENUCOORD[0] > 0)
            {
                MENUCOORD[0] -= 1;
                _last_slot = MENUCOORD[0];
                MOVENOISE = 1;
            }
        }
        else if (down_p())
        {
            _last_btn = clamp(_last_btn, 0, max_btn - 3);
            MENUCOORD[0] = 3 + _last_btn;
            MOVENOISE = 1;
        }
        else if (up_p())
        {
            if (MENUCOORD[0] > 0)
            {
                MENUCOORD[0] -= 1;
                _last_slot = MENUCOORD[0];
                MOVENOISE = 1;
            }
        }
    }
    else
    {
        _last_btn = clamp(MENUCOORD[0] - 3, 0, max_btn - 3);
        if (right_p())
        {
            MOVENOISE = 1;
            if (MENUCOORD[0] < max_btn)
            {
                MENUCOORD[0] += 1;
            }
            else
            {
                MENUCOORD[0] = 3;
            }
            _last_btn = MENUCOORD[0] - 3;
        }
        else if (left_p())
        {
            MOVENOISE = 1;
            if (MENUCOORD[0] > 3)
            {
                MENUCOORD[0] -= 1;
            }
            else
            {
                MENUCOORD[0] = max_btn;
            }
            _last_btn = MENUCOORD[0] - 3;
        }
        else if (up_p())
        {
            MENUCOORD[0] = clamp(_last_slot, 0, 2);
            MOVENOISE = 1;
        }
        else if (down_p())
        {
            MOVENOISE = 1;
            if (MENUCOORD[0] < max_btn)
            {
                MENUCOORD[0] += 1;
            }
            else
            {
                MENUCOORD[0] = 3;
            }
            _last_btn = MENUCOORD[0] - 3;
        }
    }
    if (global.is_console)
    {
        if (MENUCOORD[0] == 7)
        {
            MENUCOORD[0] = memloc;
            MOVENOISE = false;
        }
    }
    if (button1_p() && ONEBUFFER < 0)
    {
        MESSAGETIMER = -1;
        if (MENUCOORD[0] <= 2)
        {
            MENUCOORD[1] = 0;
            ONEBUFFER = 1;
            TWOBUFFER = 1;
            MENU_NO = 1;
            SELNOISE = 1;
        }
        if (MENUCOORD[0] == 3)
        {
            MENUCOORD[2] = 0;
            ONEBUFFER = 1;
            TWOBUFFER = 1;
            MENU_NO = 2;
            SELNOISE = 1;
        }
        if (MENUCOORD[0] == 4)
        {
            MENUCOORD[5] = 0;
            ONEBUFFER = 1;
            TWOBUFFER = 1;
            MENU_NO = 5;
            SELNOISE = 1;
        }
        if (MENUCOORD[0] == 5)
        {
            input_enabled = false;
            SELNOISE = 1;
            snd_free_all();
            alarm[0] = round(30 * global.fps_scale);
        }
        if (MENUCOORD[0] == 6)
        {
            scr_change_language();
            scr_84_load_ini();
            ONEBUFFER = 2;
            TWOBUFFER = 2;
            SELNOISE = 1;
        }
        if (MENUCOORD[0] == 7)
        {
            ONEBUFFER = 2;
            TWOBUFFER = 2;
            SELNOISE = 1;
            game_end();
        }
    }
}
if (OBMADE == 1)
{
    OB_DEPTH += 1;
    obacktimer += OBM;
    if (obacktimer >= 20)
    {
        DV = instance_create(0, 0, DEVICE_OBACK_4);
        DV.depth = 5 + OB_DEPTH;
        DV.OBSPEED = 0.01 * OBM;
        if (OB_DEPTH >= 60000)
        {
            OB_DEPTH = 0;
        }
        obacktimer = 0;
    }
}
if (MOVENOISE == 1)
{
    snd_play(snd_menumove);
    MOVENOISE = 0;
}
if (SELNOISE == 1)
{
    snd_play(snd_select);
    SELNOISE = 0;
}
if (BACKNOISE == 1)
{
    snd_play(snd_swing);
    BACKNOISE = 0;
}
if (DEATHNOISE == 1)
{
    snd_play(AUDIO_APPEARANCE);
    DEATHNOISE = 0;
}
ONEBUFFER -= (1 / global.fps_scale);
TWOBUFFER -= (1 / global.fps_scale);

enum UnknownEnum
{
    Value_0,
    Value_2 = 2,
    Value_29 = 29
}
