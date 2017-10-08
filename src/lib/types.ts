/**
 * login数据结构
 *
 * @interface
 */
export interface InputType {
  /**
   * 用户名
   */
  user: string,
  /**
   * 密码
   */
  password: string,
  /**
   * 版本
   */
  version: string,
  /**
   * 目录
   */
  dir: string,
  /**
   * 文件名
   */
  filename: string
}
