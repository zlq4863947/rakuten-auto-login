import { MarketSpeed } from './marketspeed';
import { InputParams } from './types';

describe('楽天証券自動ログイン', () => {
  it('自動ログインテスト', async () => {
    const input: InputParams = {
      user: 'idxxx',
      password: 'passxxx',
    };
    const marketSpeed = new MarketSpeed(input);
    // const res = marketSpeed.login();
    const res = marketSpeed.startRSS();
    expect(res).toBeTruthy();
  });
});
