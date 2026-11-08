void butt_check(){
  c_millis = millis();
  
  for (int i = 0; i < num_of_butts; i++){
    
    if ((c_millis-last_butt_db[i]) > butt_db_delay){

      pin_val[i] = digitalRead(pins_to_set[i]);
          
      if (pin_val[i]!= prev_pin_val[i]) {

        char the_butt = current_butt[i];
        if (!pin_val[i]){
            Keyboard.press(the_butt);
        }
        else{
            Keyboard.release(the_butt);
        }
        last_butt_db[i] = c_millis;
        prev_pin_val[i] = pin_val[i];
      }
    }
  }
}
