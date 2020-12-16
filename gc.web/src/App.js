import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState, useEffect, useRef } from 'react';
import { Container, Row, Col, Button, ButtonGroup } from 'react-bootstrap';
import Spinner from 'react-bootstrap/Spinner'

function App() {
  const [ connection, setConnection ] = useState(null);
  const [ challenge, setChallenge ] = useState('');
  const [ game, setGame ] = useState({userScore: 0});
  const [ clients, setClients ] = useState(0);
  const [ disabled, setDisabled ] = useState(true);

  useEffect(() => {
    const con = new HubConnectionBuilder()
        .withUrl('https://localhost:8195/game')
        .withAutomaticReconnect()
        .build();

    setConnection(con);
}, []);

useEffect(() => {
    if (connection){
        connection.start()
            .then(r => {
                connection.on('NewChallenge', state => {
                    setChallenge(state);
                    setDisabled(false);
                });

                connection.on('OnMessageCheck', state => {
                    setGame(state);
                });

                connection.on('ClientCounterChanged', state => {
                    setClients(state);
                });
            })
            .catch(e => console.log('Connection failed: ', e));
    }
}, [connection]);

const sendAnswer = async (answer) => {
    if (connection.connectionStarted){
        await connection.invoke('OnMessageReceived', { Answer: answer, UserScore: game.userScore});
    }
}

const resetState = async () => {
    if (connection.connectionStarted){
        await connection.invoke('SendNewChallenge');
    }
}

const background = {
  backgroundColor: '#282c34',
  color: 'white'
}

function renderButtons(challenge){
  if (challenge === ''){
    if (!connection || !connection.connectionStarted){
      return(<a>Connecting...</a>)
    }
    return (
    <ButtonGroup aria-label="Basic example">
      <Button variant="outline-light" onClick={resetState}>Start</Button>
    </ButtonGroup>);
  }else{
    if (disabled){
      return(<Spinner animation="grow" />);
    }else{
      return (
      <ButtonGroup aria-label="Basic example">
        <Button variant="outline-light" onClick={() => {sendAnswer(0); setDisabled(true);}} disabled={disabled}>Correct</Button>
        <Button variant="outline-light" onClick={() => {sendAnswer(1); setDisabled(true);}} disabled={disabled}>Not Correct</Button>
      </ButtonGroup>)
    };
  }
}

return (
    <div className="App">
      <header style={background}>
        <Container>
          <Row>
            <Col>Clients: {clients}</Col>
            <Col>Score: {game.userScore}</Col>
          </Row>
        </Container>
      </header>
      <header className="App-header">
      <h1>{challenge}</h1>
        {renderButtons(challenge)}
      </header>
    </div>
  );
}

export default App;
