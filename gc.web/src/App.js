import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState, useEffect, useRef } from 'react';
import { Container, Row, Col, Button, ButtonGroup } from 'react-bootstrap';
import Spinner from 'react-bootstrap/Spinner'

function App() {
  const [ connection, setConnection ] = useState(null);
  const [ challenge, setChallenge ] = useState('');
  const [ game, setGame ] = useState({userScore: 0});
  const [ disabled, setDisabled ] = useState(true);
  const [ scoreList, setScoreList] = useState([]);
  const [ nickname, setNickname] = useState('');

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

                connection.on('ClientDisconnected', connectionId => {
                    var sl = scoreList;
                });

                connection.on('ScoreChange', state => {
                    setScoreList(state);
                });

                connection.on('SetNickname', name => {
                  setNickname(name);
                })
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

const font = {
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
      <header style={font}>
        <Container>
          <Row>
            <Col>{nickname}</Col>
            <Col>Score: {game.userScore}</Col>
          </Row>
        </Container>
      </header>
      <Container className="App-content">
      <h1>{challenge}</h1>
        {renderButtons(challenge)}
        <Container style={{fontSize: 15, paddingTop: 50 }}>
        { Object.keys(scoreList).map((client, i) => {
            return <Row className="justify-content-md-center"><Col xs="2" style={font}>{scoreList[i].nickname} : </Col><Col xs="1">{scoreList[i].score}</Col></Row>;
          })}
        </Container>
      </Container>
    </div>
  );
}

export default App;
