void kpd_check(){

  if (current_kpd.getKeys())
  {
      for (int i=0; i<LIST_MAX; i++)   // Scan the whole key list.
      {
          if ( current_kpd.key[i].stateChanged )   // Only find keys that have changed state.
          {
              switch (current_kpd.key[i].kstate) {  // Report active key state : IDLE, PRESSED, HOLD, or RELEASED
                  case PRESSED:
                  Keyboard.press(current_kpd.key[i].kchar);
              break;
                  case RELEASED:
                  Keyboard.release(current_kpd.key[i].kchar);
              }
          }
      }
  }
}
