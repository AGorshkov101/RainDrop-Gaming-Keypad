#include <Keyboard.h>
#include <Keypad.h>
#include <EEPROM.h>
#include <math.h> 
//#include <HID.h>

/////////////////////////////// Pin Setup //////////////////////////////
#define layer_butt_pin A3

#define led1_pin  16
#define led2_pin  10
#define led3_pin  8
#define led4_pin  A2

#define jb1_pin 14
#define jb_space_pin 15
#define joy_x_pin A1  
#define joy_y_pin A0  

byte pins_to_set[] = {jb1_pin, jb_space_pin}; //, top1_pin, top2_pin};
byte num_of_butts = (sizeof(pins_to_set) / sizeof(pins_to_set[0]));

const byte ROWS = 5;
const byte COLS = 4; 

byte rowPins[ROWS] = {7,6,5,4,3};//{3,4,5,6,7}; //connect to the row pinouts of the kpd1 
byte colPins[COLS] = {2, 0, 1, 9}; //connect to the column pinouts of the kpd1

char modKeys [] = {
  KEY_LEFT_CTRL      , // 0   128
  KEY_LEFT_SHIFT     , // 1   129
  KEY_LEFT_ALT       , // 2   130
  KEY_LEFT_GUI       , // 3   131
  KEY_RIGHT_CTRL     , // 4   132
  KEY_RIGHT_SHIFT    , // 5   133
  KEY_RIGHT_ALT      , // 6   134
  KEY_RIGHT_GUI      , // 7   135
  KEY_UP_ARROW       , // 8   218
  KEY_DOWN_ARROW     , // 9   217
  KEY_LEFT_ARROW     , // 10  216
  KEY_RIGHT_ARROW    , // 11  215
  KEY_BACKSPACE      , // 12  178
  KEY_TAB            , // 13  179
  KEY_RETURN         , // 14  176
  KEY_ESC            , // 15  177
  KEY_INSERT         , // 16  209
  KEY_DELETE         , // 17  212
  KEY_PAGE_UP        , // 18  211
  KEY_PAGE_DOWN      , // 19  214
  KEY_HOME           , // 20  210
  KEY_END            , // 21  213
  KEY_CAPS_LOCK      , // 22  193
  KEY_F1             , // 23  194
  KEY_F2             , // 24  195
  KEY_F3             , // 25  196
  KEY_F4             , // 26  197
  KEY_F5             , // 27  198
  KEY_F6             , // 28  199
  KEY_F7             , // 29  200
  KEY_F8             , // 30  201
  KEY_F9             , // 31  202
  KEY_F10            , // 32  203
  KEY_F11            , // 33  204
  KEY_F12            , // 34  205
  '\0'                 // 35  0
};


///////////////////////// Keybind Setup ////////////////////// 

///// LAYER 1........................................
      
char keys1[ROWS][COLS] = {      
    {EEPROM.read(0),     EEPROM.read(1),      EEPROM.read(2),      EEPROM.read(3)},
    {EEPROM.read(4),     EEPROM.read(5),      EEPROM.read(6),      EEPROM.read(7)},
    {EEPROM.read(8),     EEPROM.read(9),      EEPROM.read(10),      EEPROM.read(11)},
    {EEPROM.read(12),     EEPROM.read(13),      EEPROM.read(14),      EEPROM.read(15)},
    {EEPROM.read(16),     EEPROM.read(17),      EEPROM.read(18),      EEPROM.read(19)}
};      

char jb1_1 = EEPROM.read(20);     
char jb_space_1 = EEPROM.read(21);

char top1_1 = EEPROM.read(22);      
char top2_1 = EEPROM.read(23);         
      
char joy_up_1 = EEPROM.read(24);      
char joy_down_1 = EEPROM.read(25);      
char joy_fw_1 = EEPROM.read(26);      
char joy_bw_1 = EEPROM.read(27);;      

     
///// LAYER 2........................................

char keys2[ROWS][COLS] = {      
    {EEPROM.read(28),     EEPROM.read(29),      EEPROM.read(30),      EEPROM.read(31)},
    {EEPROM.read(32),     EEPROM.read(33),      EEPROM.read(34),      EEPROM.read(35)},
    {EEPROM.read(36),     EEPROM.read(37),      EEPROM.read(38),      EEPROM.read(39)},
    {EEPROM.read(40),     EEPROM.read(41),      EEPROM.read(42),      EEPROM.read(43)},
    {EEPROM.read(44),     EEPROM.read(45),      EEPROM.read(46),      EEPROM.read(47)}
};      

char jb1_2 = EEPROM.read(48);     
char jb_space_2 = EEPROM.read(49); 
      
char top1_2 = EEPROM.read(50);      
char top2_2 = EEPROM.read(51);      
      
      
char joy_up_2 = EEPROM.read(52);       
char joy_down_2 = EEPROM.read(53);      
char joy_fw_2 = EEPROM.read(54);      
char joy_bw_2 = EEPROM.read(55);      

///// LAYER 3........................................

char keys3[ROWS][COLS] = {      
    {EEPROM.read(56),     EEPROM.read(57),      EEPROM.read(58),      EEPROM.read(59)},
    {EEPROM.read(60),     EEPROM.read(61),      EEPROM.read(62),      EEPROM.read(63)},
    {EEPROM.read(64),     EEPROM.read(65),      EEPROM.read(66),      EEPROM.read(67)},
    {EEPROM.read(68),     EEPROM.read(69),      EEPROM.read(70),      EEPROM.read(71)},
    {EEPROM.read(72),     EEPROM.read(73),      EEPROM.read(74),      EEPROM.read(75)}
};     
 
char jb1_3 = EEPROM.read(76);     
char jb_space_3 = EEPROM.read(77); 
      
char top1_3 = EEPROM.read(78);      
char top2_3 = EEPROM.read(79); 
      
char joy_up_3 = EEPROM.read(80);      
char joy_down_3 = EEPROM.read(81);      
char joy_fw_3 = EEPROM.read(82);      
char joy_bw_3 = EEPROM.read(83);      
     

///// LAYER 4........................................

char keys4[ROWS][COLS] = {
    {EEPROM.read(84),     EEPROM.read(85),      EEPROM.read(86),      EEPROM.read(87)},
    {EEPROM.read(88),     EEPROM.read(89),      EEPROM.read(90),      EEPROM.read(91)},
    {EEPROM.read(92),     EEPROM.read(93),      EEPROM.read(94),      EEPROM.read(95)},
    {EEPROM.read(96),     EEPROM.read(97),      EEPROM.read(98),      EEPROM.read(99)},
    {EEPROM.read(100),     EEPROM.read(101),      EEPROM.read(102),      EEPROM.read(103)}
};

char jb1_4 = EEPROM.read(104);
char jb_space_4 = EEPROM.read(105);

char top1_4 = EEPROM.read(106);
char top2_4 = EEPROM.read(107);

char joy_up_4 = EEPROM.read(108);
char joy_down_4 = EEPROM.read(109);
char joy_fw_4 = EEPROM.read(110);
char joy_bw_4 = EEPROM.read(111);

// Joystick angle

int8_t savedJoyAdjustment = (int8_t)EEPROM.read(112);


/////////////////////// Button Variable Setup /////////////////////////

byte pin_val[] = {0,0,0,0,0};
byte prev_pin_val[] = {0,0,0,0,0};

int butt_db_delay = 20;
unsigned long last_butt_db[] = {0,0,0,0,0};
unsigned long c_millis = 0;

/////////////////////// Joystick Variable Setup /////////////////////////

int x_zero, y_zero;  
char joyCurrentState = '\0';
char joyPreviusState = '\0';
                                            
//////////////////////////// joystick array /////////////////////////////

char butt_char_1[] = {jb1_1, jb_space_1};//, top1_1, top2_1};
char butt_char_2[] = {jb1_2, jb_space_2};//, top1_2, top2_2};
char butt_char_3[] = {jb1_3, jb_space_3};//, top1_3, top2_3};
char butt_char_4[] = {jb1_4, jb_space_4};//, top1_4, top2_4};

//////////////////////////// Define keypads /////////////////////////////

Keypad kpd1 = Keypad( makeKeymap(keys1), rowPins, colPins, ROWS, COLS );
Keypad kpd2 = Keypad( makeKeymap(keys2), rowPins, colPins, ROWS, COLS );
Keypad kpd3 = Keypad( makeKeymap(keys3), rowPins, colPins, ROWS, COLS );
Keypad kpd4 = Keypad( makeKeymap(keys4), rowPins, colPins, ROWS, COLS );

//////////////////////////// High level arrays /////////////////////////////

Keypad kpd_matrix[] = {kpd1,kpd2,kpd3,kpd4};
char* butt_matrix[] = {butt_char_1,butt_char_2,butt_char_3,butt_char_4};
char joy_matrix[4][4] = {
  {joy_up_1, joy_down_1, joy_fw_1, joy_bw_1},
  {joy_up_2, joy_down_2, joy_fw_2, joy_bw_2},
  {joy_up_3, joy_down_3, joy_fw_3, joy_bw_3},
  {joy_up_4, joy_down_4, joy_fw_4, joy_bw_4}
  };

///////////////////////// layer select setup //////////////////////////////

int layer_db_delay = 200;
unsigned long last_layer_db = 0;

const int layer_adrs = 150;
const int total_layers = 4;
byte selected_layer = 0;

int led_state[4][4] = {  
  {HIGH,LOW,LOW,LOW},
  {HIGH,HIGH,LOW,LOW},
  {HIGH,HIGH,HIGH,LOW},
  {HIGH,HIGH,HIGH,HIGH}
};

Keypad current_kpd = kpd_matrix[selected_layer];
char* current_butt = butt_matrix[selected_layer];

///// Initialize /////////////////////////////////////////////////////////////////////////////////
void setup(){
  Serial.begin(9600);
  pin_setup();
  saved_layer();
    
}

///// Main Loop //////////////////////////////////////////////////////////////////////////////////////
void loop(){
  layer_select();
  kpd_check();
  butt_check();  
  joy_check(joy_x_pin, joy_y_pin, x_zero, y_zero, & joyCurrentState,& joyPreviusState);
  PC_Communication();
}
