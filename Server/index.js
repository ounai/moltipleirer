'use strict';

const dgram = require('dgram');
const server = dgram.createSocket('udp4');

const PORT = 31337;

let players = {
  'test': {
    id: 0,
    x: 100,
    y: 100
  }
};

function addPlayer(rinfo) {
  players[`${rinfo.address}:${rinfo.port}`] = {
    id: Object.keys(players).length,
    x: 0,
    y: 0
  };
}

function removePlayer(rinfo) {
  delete players[`${rinfo.address}:${rinfo.port}`];
}

function getPlayer(rinfo) {
  return players[`${rinfo.address}:${rinfo.port}`] || null;
}

function movePlayer(rinfo, x, y) {
  const key = `${rinfo.address}:${rinfo.port}`;

  players[key] = { ...players[key], x, y };
}

function sendPacket(packet, rinfo, callback) {
  if (!callback) {
    callback = err => {
      if (err) throw err;
      else console.log('Sent', packet, 'to', `${rinfo.address}:${rinfo.port}`);
    };
  }

  server.send(packet, rinfo.port, rinfo.address, callback);
}

function createPacket(...args) {
  return args.join('|');
}

server.on('error', err => {
  console.error('Server encountered an error:', err);
  server.close();
});

server.on('message', (msg, rinfo) => {
  const packet = msg.toString('utf8');
  const player = getPlayer(rinfo);

  console.log('Server received', packet, 'from', `${rinfo.address}:${rinfo.port}`);

  if (packet === 'join') {
    sendPacket('hello', rinfo);

    for (let key in players) {
      const otherPlayer = players[key];

      sendPacket(
        createPacket('joined', otherPlayer.id, otherPlayer.x, otherPlayer.y),
        rinfo
      );
    }

    if (player === null) addPlayer(rinfo);
  } else if (packet === 'leave') {
    sendPacket('goodbye', rinfo);

    for (let otherPlayer in players) {
      sendPacket(
        createPacket('left', otherPlayer.id),
        rinfo
      );
    }

    if (player !== null) removePlayer(rinfo);
  }
});

server.on('listening', () => {
  const address = server.address();

  console.log('Server listening on', `${address.address}:${address.port}`);
});

server.bind(PORT);

