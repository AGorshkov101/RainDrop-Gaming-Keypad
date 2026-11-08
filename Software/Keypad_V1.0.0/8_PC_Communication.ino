
void PC_Communication(){

// Codex 
// Start marker "&&", memory location, delineator ":", character
// Example: &&1:y letter y at memory location 1. Use ";" instead of ":" for modifier keys. Use "@" for joystick angle.   
    
  while (Serial.available() > 0) {        

    int mem_location = 0;
    char charToPass = '\0';
        
    // Read the incoming bytes to see if its the proper data format 
    char check1 = Serial.read();
    char check2 = Serial.read();

    
    if (check1 == '?'){ // Replies to computer looking for Arduino 
      Serial.println("Polo");
    }            
    else if ((check1 == '|') or (check2 == '|')){ //uploads key char if requested      
      for (int i=0; i < 112; i++){
        Serial.println(EEPROM.read(i));
      }
      Serial.println((int8_t)EEPROM.read(112)); // handle joystick offset with negative values
      Serial.println(selected_layer); // tell PC the current player        
    } 
    else if (check1 == '%'){ //update layer to PC software layer       
      selected_layer = (int)check2 - 48;
      key_layer_update();             
    }
    else if ((check1 == '&') && (check2 == '&')){ // Change keys 

      // Interpret what comand came in 
      mem_location = Serial.parseInt();
      char inputType = Serial.read(); // Removes the delineator ":" or ";" from buffer and stores it 
      
      if (inputType == ';'){
        charToPass = modKeys [Serial.parseInt()]; // Handles special characters         
      }
      else if (inputType == '@'){
        savedJoyAdjustment = Serial.parseInt(); // Handles Joystick angle adjustment        
      }      
      else if (isDigit(Serial.peek())){
        int tempInt = Serial.parseInt();
        charToPass = tempInt + '0'; //Convert a number to a character 
      }
      else{
        charToPass = Serial.read();
      }

      // Save to memory 
      if (inputType == '@'){
        EEPROM.write(mem_location, savedJoyAdjustment);         
      }
      else{
        EEPROM.write(mem_location, charToPass);
      }
      

//////////////////// set keys equal to new memory without restarting

      int IntendedKeypadLayer = 3;
      int layerMemSize = 28; 

      if (mem_location < layerMemSize){
        IntendedKeypadLayer = 0;        
      }
      else if(mem_location < (layerMemSize * 2)){
        IntendedKeypadLayer = 1;
      }
      else if(mem_location < (layerMemSize * 3)){
        IntendedKeypadLayer = 2;
      }

      int intendedKeypadCluster = mem_location - (layerMemSize*IntendedKeypadLayer); 

      //keypad = 0-19
      //btns = 20-23
      //joy = 24-27

      if (intendedKeypadCluster <= 19){
        //keypad = 0-19
        
        int matrixX = 4;
        int matrixY = 3;
        int rowSize = 4;

        if(intendedKeypadCluster <= 3){
          matrixX = 0;
          matrixY = intendedKeypadCluster; 
        }
        else if(intendedKeypadCluster <= 7){
          matrixX = 1;
          matrixY = intendedKeypadCluster - rowSize;
        }
        else if(intendedKeypadCluster <= 11){
          matrixX = 2;
          matrixY = intendedKeypadCluster - (rowSize*2);
        }
        else if(intendedKeypadCluster <= 15){
          matrixX = 3;
          matrixY = intendedKeypadCluster - (rowSize*3);
        }
        else if(intendedKeypadCluster <= 19){
          matrixX = 4;
          matrixY = intendedKeypadCluster - (rowSize*4);
        }

        switch(IntendedKeypadLayer){
          case 0:
            keys1[matrixX][matrixY] = charToPass; 
            break;
          case 1:
            keys2[matrixX][matrixY] = charToPass; 
            break;
          case 2:
            keys3[matrixX][matrixY] = charToPass; 
            break;
          case 3:
            keys4[matrixX][matrixY] = charToPass; 
            break;
        }
        //Serial.print("matrixX: ");
        //Serial.println(matrixX);
        //Serial.print("matrixY: ");
        //Serial.println(matrixY); 
        //keys1[0][0] = charToPass;         
      }
      else if(intendedKeypadCluster <= 23){
        //btns = 20-23              
        switch(IntendedKeypadLayer){
          case 0:
            //char butt_char_1[] = {jb1_1, jb_space_1, top1_1, top2_1};
            butt_char_1[(intendedKeypadCluster - 20)] = charToPass;             
            break;
          case 1:
            butt_char_2[(intendedKeypadCluster - 20)] = charToPass; 
            break;
          case 2:
            butt_char_3[(intendedKeypadCluster - 20)] = charToPass; 
            break;
          case 3:
            butt_char_4[(intendedKeypadCluster - 20)] = charToPass; 
            break;
        }        
      }
      else if(intendedKeypadCluster <= 27){ // could just be else statment 
        joy_matrix[IntendedKeypadLayer][intendedKeypadCluster - 24] = charToPass;
      }

//      Serial.print("mem_location: ");
//      Serial.println(mem_location);
//      Serial.print("charToPass: ");
//      Serial.println(charToPass);
//      
//      Serial.print("IntendedKeypadLayer: ");
//      Serial.println(IntendedKeypadLayer);      
//      Serial.print("intendedKeypadCluster: ");
//      Serial.println(intendedKeypadCluster);
//      Serial.println(" ");
    
    }      
  }
}
