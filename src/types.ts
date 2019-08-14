/**
 * login数据结构
 *
 * @interface
 */
export interface InputParams {
  /**
   * 用户名
   */
  user: string;
  /**
   * 密码
   */
  password: string;
  /**
   * 目录, 默认: "C:/Program Files (x86)/MarketSpeed/MarketSpeed"
   */
  dir?: string;
  /**
   * 文件名, 默认: "MarketSpeed.exe"
   */
  filename?: string;
}
