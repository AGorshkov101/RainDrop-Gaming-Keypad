void pin_setup(){
  
  //////////////////////// layer selector ////////////////////
  pinMode(layer_butt_pin, INPUT_PULLUP);  

  //LED start
  pinMode(led1_pin, OUTPUT);  
  digitalWrite(led1_pin, led_state[selected_layer][0]);
  pinMode(led2_pin, OUTPUT);  
  digitalWrite(led2_pin, led_state[selected_layer][1]);
  pinMode(led3_pin, OUTPUT);  
  digitalWrite(led3_pin, led_state[selected_layer][2]);
  pinMode(led4_pin, OUTPUT);  
  digitalWrite(led4_pin, led_state[selected_layer][3]);

  ////////////////////////////// buttons ///////////////////
  for (int i=0; i<num_of_butts; i++){
    pinMode(pins_to_set[i], INPUT_PULLUP);
  }

  /////////////////////////////// Joystick ///////////////////////
  pinMode(joy_x_pin, INPUT);  
  pinMode(joy_y_pin, INPUT);
  
  delay(250);  // short delay to let outputs settle
  x_zero = analogRead(joy_x_pin);  // get the initial values
  y_zero = analogRead(joy_y_pin);  // Joystick should be in neutral position when reading these

}
